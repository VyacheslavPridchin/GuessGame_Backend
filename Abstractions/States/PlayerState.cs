namespace GuessGame.Abstractions.States;

[Serializable]
[GenerateSerializer]
public class PlayerState
{
    public PlayerState() { }

    public PlayerState(string name)
    {
        ID = Guid.NewGuid();
        Nickname = name;
    }

    public PlayerState(string name, Guid id)
    {
        ID = id;
        Nickname = name;
    }

    [Id(0)]
    public Guid ID { get; set; } = Guid.Empty;
    [Id(1)]
    public string Nickname { get; set; } = string.Empty;
}