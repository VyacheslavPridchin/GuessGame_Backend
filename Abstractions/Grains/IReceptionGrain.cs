using GuessGame.Abstractions.States;

namespace GuessGame.Abstractions.Grains;

public interface IReceptionGrain : IGrainWithIntegerKey
{
    public Task<Guid> CheckIn(string nickname);
    public Task CheckOut(Guid id);
    public Task<Dictionary<Guid, PlayerState>> GetPlayers();
    public Task<Dictionary<Guid, long>> GetScore();
    public Task<PlayerState> GetPlayer(Guid id);
    public Task AddScore(Guid id);
}