
namespace GuessGame.Silo.States;

[Serializable]
[GenerateSerializer]
public class ScoreState
{
    [Id(0)]
    public Dictionary<Guid, long> PlayersScore { get; private set; } = new Dictionary<Guid, long>();
}

