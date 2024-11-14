public class ChessGameState
{
    public Board Board { get; set; } = new Board();
}

public class Board
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<Piece> Pieces { get; set; } = new List<Piece>();
}

public class Piece
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public Position Position { get; set; } = new Position();
    public string Color { get; set; } = "";
}

public class Position
{
    public int Row { get; set; }
    public int Column { get; set; }
}