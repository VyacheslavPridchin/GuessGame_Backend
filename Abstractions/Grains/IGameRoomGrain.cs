namespace GuessGame.Abstractions.Grains;

public interface IGameRoomGrain : IGrainWithGuidKey
{
    public Task Initialize();
    public Task Join(Guid id);
    public Task<List<Guid>> GetPlayers();
    public Task<Dictionary<Guid, int>> GetAnswers();
    public Task GuessNumber(Guid id, int number);
    public Task<string> GetCurrentAction();
    public Task<List<Guid>> GetWinners();
    public Task<int> GetTrueNumber();
}
