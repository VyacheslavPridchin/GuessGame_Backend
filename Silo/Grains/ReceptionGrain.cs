using GuessGame.Abstractions.Grains;
using GuessGame.Abstractions.States;
using GuessGame.Silo.States;

namespace GuessGame.Silo.Grains;

public class ReceptionGrain : Grain, IReceptionGrain
{
    private readonly IPersistentState<ReceptionState> receptionState;
    private readonly IPersistentState<ScoreState> scoreState;

    public Dictionary<Guid, PlayerState> PlayerStates = new Dictionary<Guid, PlayerState>();

    public ReceptionGrain([PersistentState("ReceptionState")] IPersistentState<ReceptionState> receptionState, [PersistentState("ScoreState")] IPersistentState<ScoreState> scoreState)
    {
        this.receptionState = receptionState;
        this.scoreState = scoreState;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        PlayerStates.Clear();

        foreach (var item in receptionState.State.PlayerIds)
            PlayerStates.Add(item.Value, new PlayerState(item.Key, item.Value));

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<Guid> CheckIn(string nickname)
    {
        if (receptionState.State.PlayerIds.ContainsKey(nickname))
        {
            var ps = new PlayerState(nickname, receptionState.State.PlayerIds[nickname]);
            PlayerStates[ps.ID] = ps;
        }
        else
        {
            var ps = new PlayerState(nickname);
            PlayerStates[ps.ID] = ps;
            receptionState.State.PlayerIds[nickname] = ps.ID;
            scoreState.State.PlayersScore[ps.ID] = 0;
            await receptionState.WriteStateAsync();
            await scoreState.WriteStateAsync();
        }

        return receptionState.State.PlayerIds[nickname];
    }

    public Task CheckOut(Guid id)
    {
        PlayerStates.Remove(id);
        return Task.CompletedTask;
    }

    public Task<Dictionary<Guid, long>> GetScore() => Task.FromResult(scoreState.State.PlayersScore);

    public Task<Dictionary<Guid, PlayerState>> GetPlayers() => Task.FromResult(PlayerStates);

    public Task<PlayerState> GetPlayer(Guid id)
    {
        var playerState = PlayerStates[id];

        return Task.FromResult(playerState);
    }

    public async Task AddScore(Guid id)
    {
        if (scoreState.State.PlayersScore.ContainsKey(id))
            scoreState.State.PlayersScore[id]++;
        else
            scoreState.State.PlayersScore[id] = 1;

        await scoreState.WriteStateAsync();
    }
}