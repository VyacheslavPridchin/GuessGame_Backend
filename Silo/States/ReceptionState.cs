namespace GuessGame.Silo.States;

[Serializable]
[GenerateSerializer]
public class ReceptionState
{
    [Id(0)]
    public Dictionary<string, Guid> PlayerIds { get; private set; } = new Dictionary<string, Guid>();
}