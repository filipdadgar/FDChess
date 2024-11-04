using System.Text.Json.Serialization;

namespace FDChess.Model
{
    public class Pawn : Piece
    {
        public Pawn() : base() { }

        [JsonConstructor]
        public Pawn(int id, Position position, string color)
            : base(id, "Pawn", position, color) { }

        public override bool IsMoveValid(Position newPosition, Board board)
        {
            int rowDiff = newPosition.Row - Position.Row;
            int colDiff = newPosition.Column - Position.Column;

            if (Color == "White")
            {
                // One square forward or two on the first move
                if ((rowDiff == 1 || (Position.Row == 1 && rowDiff == 2)) && colDiff == 0)
                {
                    return !board.IsPositionOccupied(newPosition);
                }
            }
            else
            {
                // One square forward or two on the first move
                if ((rowDiff == -1 || (Position.Row == 6 && rowDiff == -2)) && colDiff == 0)
                {
                    return !board.IsPositionOccupied(newPosition);
                }
            }

            // Add diagonal capture logic here
            return false;
        }
    }


    public class Rook : Piece
    {
        public Rook() : base() { }

        [JsonConstructor]
        public Rook(int id, Position position, string color) 
            : base(id, "Rook", position, color) { }

        public override bool IsMoveValid(Position newPosition, Board board)
        {
            // Check if the move is horizontal or vertical
            if (newPosition.Row == Position.Row || newPosition.Column == Position.Column)
            {
                // Check if the path is clear (no pieces blocking the way)
                return board.IsPathClear(Position, newPosition);
            }
            return false;
        }
    }

    public class Knight : Piece
    {
        public Knight() : base() { }

        [JsonConstructor]
        public Knight(int id, Position position, string color) 
            : base(id, "Knight", position, color) { }

        public override bool IsMoveValid(Position newPosition, Board board)
        {
            int rowDiff = Math.Abs(newPosition.Row - Position.Row);
            int colDiff = Math.Abs(newPosition.Column - Position.Column);

            // Check if the move is L-shaped
            return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
        }
    }

    public class Bishop : Piece
    {
        public Bishop() : base() { }

        [JsonConstructor]
        public Bishop(int id, Position position, string color) 
            : base(id, "Bishop", position, color) { }

        public override bool IsMoveValid(Position newPosition, Board board)
        {
            // Check if the move is diagonal
            int rowDiff = Math.Abs(newPosition.Row - Position.Row);
            int colDiff = Math.Abs(newPosition.Column - Position.Column);

            return rowDiff == colDiff && board.IsPathClear(Position, newPosition);
        }
    }

    public class Queen : Piece
    {
        public Queen() : base() { }
        [JsonConstructor]
        public Queen(int id, Position position, string color) 
            : base(id, "Queen", position, color) { }

        public override bool IsMoveValid(Position newPosition, Board board)
        {
            // Check if the move is horizontal, vertical, or diagonal
            int rowDiff = Math.Abs(newPosition.Row - Position.Row);
            int colDiff = Math.Abs(newPosition.Column - Position.Column);

            if (newPosition.Row == Position.Row || newPosition.Column == Position.Column)
            {
                return board.IsPathClear(Position, newPosition);
            }
            return rowDiff == colDiff && board.IsPathClear(Position, newPosition);
        }
    }

    public class King : Piece
    {
        public King() : base() { }

        [JsonConstructor]
        public King(int id, Position position, string color) 
            : base(id, "King", position, color) { }

        public override bool IsMoveValid(Position newPosition, Board board)
        {
            int rowDiff = Math.Abs(newPosition.Row - Position.Row);
            int colDiff = Math.Abs(newPosition.Column - Position.Column);

            // Check if the move is one square in any direction
            return rowDiff <= 1 && colDiff <= 1;
        }
    }

}