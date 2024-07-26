using Orleans;

namespace GuessGame.Abstractions.Grains;

public interface IQueueGrain : IGrainWithIntegerKey
{
    public Task Join(Guid id);
    public Task Leave(Guid id);
    public Task<List<Guid>> GetQueue();
    public Task<Guid> GetInvitation(Guid id);
}
