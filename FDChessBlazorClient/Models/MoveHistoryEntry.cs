public class MoveHistoryEntry
{
    public string Piece { get; set; } = "";
    public string Color { get; set; } = "";
    public Position From { get; set; } = new Position();
    public Position To { get; set; } = new Position();
    public string Message { get; set; } = "";
}