namespace FDChess.Model;

public class MoveRequest
{
    public Position CurrentPosition { get; set; }
    public Position NewPosition { get; set; }
}