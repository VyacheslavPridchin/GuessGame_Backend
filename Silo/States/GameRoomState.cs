using GuessGame.Abstractions.States;

namespace GuessGame.Silo.States;

[Serializable]
[GenerateSerializer]
public class GameRoomState : IGameRoomState
{
    public enum CurrentActions { WaitPlayers, WaitAnswers, AnnouncementResults, Complete, Interrupted }

    [Id(0)]
    public Guid ID { get; private set; } = Guid.Empty;

    [Id(1)]
    public int RandomNumber { get; private set; }

    [Id(2)]
    public DateTime TimestampStart { get; private set; } = DateTime.MinValue;

    [Id(3)]
    public DateTime TimestampEnd { get; private set; } = DateTime.MaxValue;

    [Id(4)]
    public CurrentActions CurrentAction { get; private set; } = CurrentActions.WaitPlayers;

    [Id(5)]
    public List<Guid> Players { get; private set; } = new List<Guid>();

    [Id(6)]
    public Dictionary<Guid, int> Answers { get; private set; } = new Dictionary<Guid, int>();

    [Id(7)]
    public List<Guid> Winners { get; private set; } = new List<Guid>();

    public void InitRoom(Guid id, int randomNumber)
    {
        ID = id;
        RandomNumber = randomNumber;
        CurrentAction = CurrentActions.WaitPlayers;
    }

    public void MarkStartGame()
    {
        TimestampStart = DateTime.Now;
        CurrentAction = CurrentActions.WaitAnswers;
    }

    public void MarkStopGame()
    {
        TimestampEnd = DateTime.Now;
        CurrentAction = CurrentActions.AnnouncementResults;
    }

    public bool TryRelease()
    {
        if (CurrentAction == CurrentActions.AnnouncementResults)
        {
            CurrentAction = CurrentActions.Complete;
            return true;
        }

        return false;
    }

    public void Abort()
    {
        TimestampEnd = DateTime.Now;
        CurrentAction = CurrentActions.Interrupted;
    }
}