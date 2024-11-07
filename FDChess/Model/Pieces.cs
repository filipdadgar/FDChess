// Pieces.cs
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

            if (Color == "white")
            {
                if (colDiff == 0 && (rowDiff == 1 || (Position.Row == 1 && rowDiff == 2)))
                {
                    return !board.IsPositionOccupied(newPosition);
                }
                if (Math.Abs(colDiff) == 1 && rowDiff == 1)
                {
                    return board.IsPositionOccupiedByOpponent(newPosition, Color);
                }
            }
            else
            {
                if (colDiff == 0 && (rowDiff == -1 || (Position.Row == 6 && rowDiff == -2)))
                {
                    return !board.IsPositionOccupied(newPosition);
                }
                if (Math.Abs(colDiff) == 1 && rowDiff == -1)
                {
                    return board.IsPositionOccupiedByOpponent(newPosition, Color);
                }
            }

            return false;
        }

        public override List<Position> GetPossibleMoves(Board board)
        {
            var possibleMoves = new List<Position>();
            int direction = Color == "white" ? 1 : -1;

            // Single step forward
            var newPosition = new Position(Position.Row + direction, Position.Column);
            if (!board.IsPositionOccupied(newPosition))
            {
                possibleMoves.Add(newPosition);
            }

            // Double step forward from initial position
            if ((Color == "white" && Position.Row == 1) || (Color == "black" && Position.Row == 6))
            {
                newPosition = new Position(Position.Row + 2 * direction, Position.Column);
                if (!board.IsPositionOccupied(newPosition))
                {
                    possibleMoves.Add(newPosition);
                }
            }

            // Capture diagonally
            newPosition = new Position(Position.Row + direction, Position.Column - 1);
            if (board.IsPositionOccupiedByOpponent(newPosition, Color))
            {
                possibleMoves.Add(newPosition);
            }

            newPosition = new Position(Position.Row + direction, Position.Column + 1);
            if (board.IsPositionOccupiedByOpponent(newPosition, Color))
            {
                possibleMoves.Add(newPosition);
            }

            return possibleMoves;
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
            if (newPosition.Row == Position.Row || newPosition.Column == Position.Column)
            {
                return board.IsPathClear(Position, newPosition);
            }
            return false;
        }

        public override List<Position> GetPossibleMoves(Board board)
        {
            var possibleMoves = new List<Position>();

            // Horizontal and vertical moves
            for (int i = 0; i < 8; i++)
            {
                if (i != Position.Row)
                {
                    var newPosition = new Position(i, Position.Column);
                    if (IsMoveValid(newPosition, board))
                    {
                        possibleMoves.Add(newPosition);
                    }
                }
                if (i != Position.Column)
                {
                    var newPosition = new Position(Position.Row, i);
                    if (IsMoveValid(newPosition, board))
                    {
                        possibleMoves.Add(newPosition);
                    }
                }
            }

            return possibleMoves;
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

            return (rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2);
        }

        public override List<Position> GetPossibleMoves(Board board)
        {
            var possibleMoves = new List<Position>();
            var moveOffsets = new (int, int)[]
            {
                (2, 1), (2, -1), (-2, 1), (-2, -1),
                (1, 2), (1, -2), (-1, 2), (-1, -2)
            };

            foreach (var (rowOffset, colOffset) in moveOffsets)
            {
                var newPosition = new Position(Position.Row + rowOffset, Position.Column + colOffset);
                if (IsMoveValid(newPosition, board))
                {
                    possibleMoves.Add(newPosition);
                }
            }

            return possibleMoves;
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
            int rowDiff = Math.Abs(newPosition.Row - Position.Row);
            int colDiff = Math.Abs(newPosition.Column - Position.Column);

            return rowDiff == colDiff && board.IsPathClear(Position, newPosition);
        }

        public override List<Position> GetPossibleMoves(Board board)
        {
            var possibleMoves = new List<Position>();

            // Diagonal moves
            for (int i = 1; i < 8; i++)
            {
                var newPosition = new Position(Position.Row + i, Position.Column + i);
                if (IsMoveValid(newPosition, board))
                {
                    possibleMoves.Add(newPosition);
                }

                newPosition = new Position(Position.Row + i, Position.Column - i);
                if (IsMoveValid(newPosition, board))
                {
                    possibleMoves.Add(newPosition);
                }

                newPosition = new Position(Position.Row - i, Position.Column + i);
                if (IsMoveValid(newPosition, board))
                {
                    possibleMoves.Add(newPosition);
                }

                newPosition = new Position(Position.Row - i, Position.Column - i);
                if (IsMoveValid(newPosition, board))
                {
                    possibleMoves.Add(newPosition);
                }
            }

            return possibleMoves;
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
            int rowDiff = Math.Abs(newPosition.Row - Position.Row);
            int colDiff = Math.Abs(newPosition.Column - Position.Column);

            if (newPosition.Row == Position.Row || newPosition.Column == Position.Column)
            {
                return board.IsPathClear(Position, newPosition);
            }
            return rowDiff == colDiff && board.IsPathClear(Position, newPosition);
        }

        public override List<Position> GetPossibleMoves(Board board)
        {
            var possibleMoves = new List<Position>();

            // Horizontal, vertical, and diagonal moves
            for (int i = 0; i < 8; i++)
            {
                if (i != Position.Row)
                {
                    var newPosition = new Position(i, Position.Column);
                    if (IsMoveValid(newPosition, board))
                    {
                        possibleMoves.Add(newPosition);
                    }
                }
                if (i != Position.Column)
                {
                    var newPosition = new Position(Position.Row, i);
                    if (IsMoveValid(newPosition, board))
                    {
                        possibleMoves.Add(newPosition);
                    }
                }

                var diagonalPosition1 = new Position(Position.Row + i, Position.Column + i);
                if (IsMoveValid(diagonalPosition1, board))
                {
                    possibleMoves.Add(diagonalPosition1);
                }

                var diagonalPosition2 = new Position(Position.Row + i, Position.Column - i);
                if (IsMoveValid(diagonalPosition2, board))
                {
                    possibleMoves.Add(diagonalPosition2);
                }

                var diagonalPosition3 = new Position(Position.Row - i, Position.Column + i);
                if (IsMoveValid(diagonalPosition3, board))
                {
                    possibleMoves.Add(diagonalPosition3);
                }

                var diagonalPosition4 = new Position(Position.Row - i, Position.Column - i);
                if (IsMoveValid(diagonalPosition4, board))
                {
                    possibleMoves.Add(diagonalPosition4);
                }
            }

            return possibleMoves;
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

            return rowDiff <= 1 && colDiff <= 1;
        }

        public override List<Position> GetPossibleMoves(Board board)
        {
            var possibleMoves = new List<Position>();
            var moveOffsets = new (int, int)[]
            {
                (1, 0), (-1, 0), (0, 1), (0, -1),
                (1, 1), (1, -1), (-1, 1), (-1, -1)
            };

            foreach (var (rowOffset, colOffset) in moveOffsets)
            {
                var newPosition = new Position(Position.Row + rowOffset, Position.Column + colOffset);
                if (IsMoveValid(newPosition, board))
                {
                    possibleMoves.Add(newPosition);
                }
            }

            return possibleMoves;
        }
    }
}