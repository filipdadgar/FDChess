public class MoveResponse
{
    public ChessGameState GameState { get; set; } = new ChessGameState();
    public string Message { get; set; } = "";
}