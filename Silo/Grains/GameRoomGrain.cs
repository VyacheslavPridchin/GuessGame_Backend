using GuessGame.Abstractions.Grains;
using GuessGame.Silo.States;
using Microsoft.Extensions.Configuration;

namespace GuessGame.Silo.Grains;

public class GameRoomGrain : Grain, IGameRoomGrain
{
    private readonly IPersistentState<GameRoomState> gameRoomState;
    private readonly int maxPlayers;
    private readonly Random rnd;
    private DateTime initTime;
    private IGrainTimer? timer;

    public GameRoomGrain([PersistentState("GameRoomState")] IPersistentState<GameRoomState> gameRoomState, IConfiguration configuration)
    {
        this.gameRoomState = gameRoomState;
        maxPlayers = configuration.GetValue<int>("Settings:MaxPlayers");
        rnd = new Random();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        timer = this.RegisterGrainTimer(CheckTimeout, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));

        await gameRoomState.ReadStateAsync();
    }

    public async Task Initialize()
    {
        initTime = DateTime.Now;
        gameRoomState.State.InitRoom(this.GetPrimaryKey(), rnd.Next(0, 100));
        await gameRoomState.WriteStateAsync();
    }

    public async Task Join(Guid id)
    {
        gameRoomState.State.Players.Add(id);
        await gameRoomState.WriteStateAsync();

        if (gameRoomState.State.Players.Count == maxPlayers)
        {
            gameRoomState.State.MarkStartGame();
            await gameRoomState.WriteStateAsync();
        }
    }

    public async Task GuessNumber(Guid id, int number)
    {
        gameRoomState.State.Answers[id] = number;
        await gameRoomState.WriteStateAsync();
        CheckAnswers();
    }

    public Task<Dictionary<Guid, int>> GetAnswers() => Task.FromResult(gameRoomState.State.Answers);

    public Task<List<Guid>> GetPlayers() => Task.FromResult(gameRoomState.State.Players);

    public Task<string> GetCurrentAction() => Task.FromResult(gameRoomState.State.CurrentAction.ToString());

    public Task<List<Guid>> GetWinners() => Task.FromResult(gameRoomState.State.Winners);

    public Task<int> GetTrueNumber() => Task.FromResult(gameRoomState.State.RandomNumber);

    private async Task CheckTimeout()
    {
        if ((DateTime.Now - initTime).TotalMinutes > 1 & gameRoomState.State.CurrentAction == GameRoomState.CurrentActions.WaitPlayers)
        {
            gameRoomState.State.Abort();
            await gameRoomState.WriteStateAsync();
            timer?.Dispose();
        }
    }

    private async void CheckAnswers()
    {
        Console.WriteLine($"{gameRoomState.State.Players.Count} == {maxPlayers}");

        if (gameRoomState.State.Answers.Count == maxPlayers)
        {
            int closestDifference = int.MaxValue;
            List<Guid> closestIds = new List<Guid>();

            foreach (var item in gameRoomState.State.Answers)
            {
                int difference = Math.Abs(gameRoomState.State.RandomNumber - item.Value);
                if (difference < closestDifference)
                {
                    closestDifference = difference;
                    closestIds.Clear();
                    closestIds.Add(item.Key);
                }
                else if (difference == closestDifference)
                {
                    closestIds.Add(item.Key);
                }
            }

            var reception = GrainFactory.GetGrain<IReceptionGrain>(0);

            foreach (var winnerID in closestIds)
            {
                gameRoomState.State.Winners.Add(winnerID);
                await reception.AddScore(winnerID);
            }

            gameRoomState.State.MarkStopGame();
            timer?.Dispose();
            await gameRoomState.WriteStateAsync();
        }
    }
}