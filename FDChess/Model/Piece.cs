// Piece.cs
using System.Text.Json.Serialization;


namespace FDChess.Model
{

    public abstract class Piece
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Position Position { get; set; }
        public string? Color { get; set; }
        public bool IsRemoved { get; set; } = false;
        public string RemovedBy { get; set; } = "";
        public Position? RemovedAt { get; set; }

        // Parameterless constructor for deserialization
        protected Piece()
        {
        }

        // Constructor with parameters
        [JsonConstructor]
        protected Piece(int id, string name, Position position, string color)
        {
            Id = id;
            Name = name;
            Position = position;
            Color = color;
            IsRemoved = false;
        }

        public override string ToString()
        {
            return $"{Name} at {Position}";
        }

        // Abstract method requiring implementation in derived classes
        public abstract bool IsMoveValid(Position newPosition, Board board);

        public abstract List<Position> GetPossibleMoves(Board board);

    }
}