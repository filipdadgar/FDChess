public class RemovedPiece
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
    public string RemovedBy { get; set; } = "";
    public Position RemovedAt { get; set; } = new Position();
}