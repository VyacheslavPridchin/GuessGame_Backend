
namespace GuessGame.Abstractions.States;

public interface IGameRoomState
{
    void MarkStartGame();

    public void MarkStopGame();

    public bool TryRelease();

    public void Abort();
}