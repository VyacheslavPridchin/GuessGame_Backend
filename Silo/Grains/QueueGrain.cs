using GuessGame.Abstractions.Grains;
using Microsoft.Extensions.Configuration;
using Orleans;
using System.Collections.Concurrent;
using System.Configuration;
using System.Xml.Linq;

namespace GuessGame.Silo.Grains
{
    public class QueueGrain : Grain, IQueueGrain
    {
        private class Invitation
        {
            public Invitation(Guid id)
            {
                RoomID = id;
            }
            public Guid RoomID { get; private set; }

            public List<Guid> Invited { get; private set; } = new List<Guid>();

            public bool CheckInvite(Guid id) => Invited.Contains(id);
        }

        private readonly int maxPlayers;
        private List<Invitation> invitations = new List<Invitation>();

        public ConcurrentBag<Guid> Queue { get; private set; } = new ConcurrentBag<Guid>();

        public QueueGrain(IConfiguration configuration)
        {
            maxPlayers = configuration.GetValue<int>("Settings:MaxPlayers");
        }

        public Task<List<Guid>> GetQueue()
        {
            return Task.FromResult(Queue.ToList());
        }

        public Task Join(Guid id)
        {
            Queue.Add(id);

            CheckQueue();

            return Task.CompletedTask;
        }

        public Task Leave(Guid id)
        {
            Queue = new ConcurrentBag<Guid>(Queue.Except(new[] { id }));

            return Task.CompletedTask;
        }

        public Task<Guid> GetInvitation(Guid id)
        {
            var invite = invitations.FirstOrDefault(x => x.CheckInvite(id));

            if (invite != null)
            {
                invite.Invited.Remove(id);
                if (invite.Invited.Count == 0)
                    invitations.Remove(invite);
                return Task.FromResult(invite.RoomID);
            }
            else
                return Task.FromResult(Guid.Empty);
        }

        public async void CheckQueue()
        {
            if (Queue.Count >= maxPlayers)
            {
                var players = Queue.Take(maxPlayers).ToList();

                Queue = new ConcurrentBag<Guid>(Queue.Except(players));

                var roomID = Guid.NewGuid();

                var grain = GrainFactory.GetGrain<IGameRoomGrain>(roomID);
                await grain.Initialize();

                Invitation invitation = new Invitation(roomID);
                invitation.Invited.AddRange(players);
                invitations.Add(invitation);
            }
        }
    }
}
