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
            int rowDiff = Math.Abs(newPosition.Row - Position.Row);
            int colDiff = Math.Abs(newPosition.Column - Position.Column);

            if (newPosition.Row == Position.Row || newPosition.Column == Position.Column || rowDiff == colDiff)
            {
                // Check for blocking pieces
                int rowDirection = newPosition.Row > Position.Row ? 1 : (newPosition.Row < Position.Row ? -1 : 0);
                int colDirection = newPosition.Column > Position.Column ? 1 : (newPosition.Column < Position.Column ? -1 : 0);

                int currentRow = Position.Row + rowDirection;
                int currentColumn = Position.Column + colDirection;

                while (currentRow != newPosition.Row || currentColumn != newPosition.Column)
                {
                    if (board.IsPositionOccupied(new Position(currentRow, currentColumn)))
                    {
                        return false;
                    }
                    currentRow += rowDirection;
                    currentColumn += colDirection;
                }

                var pieceAtNewPosition = board.GetPieceAtPosition(newPosition);
                if (pieceAtNewPosition == null || pieceAtNewPosition.Color != this.Color)
                {
                    return true;
                }
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

            if ((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2))
            {
                var pieceAtNewPosition = board.GetPieceAtPosition(newPosition);
                if (pieceAtNewPosition == null || pieceAtNewPosition.Color != this.Color)
                {
                    return true;
                }
            }
            return false;
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

        // Bishops move diagonally, so rowDiff must equal colDiff
        if (rowDiff == colDiff)
        {
            // Check for blocking pieces
            int rowDirection = newPosition.Row > Position.Row ? 1 : -1;
            int colDirection = newPosition.Column > Position.Column ? 1 : -1;

            int currentRow = Position.Row + rowDirection;
            int currentColumn = Position.Column + colDirection;

            while (currentRow != newPosition.Row && currentColumn != newPosition.Column)
            {
                if (board.IsPositionOccupied(new Position(currentRow, currentColumn)))
                {
                    return false;
                }
                currentRow += rowDirection;
                currentColumn += colDirection;
            }

            var pieceAtNewPosition = board.GetPieceAtPosition(newPosition);
            if (pieceAtNewPosition == null || pieceAtNewPosition.Color != this.Color)
            {
                return true;
            }
        }
        return false;
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

        if (newPosition.Row == Position.Row || newPosition.Column == Position.Column || rowDiff == colDiff)
        {
            // Check for blocking pieces
            int rowDirection = newPosition.Row > Position.Row ? 1 : (newPosition.Row < Position.Row ? -1 : 0);
            int colDirection = newPosition.Column > Position.Column ? 1 : (newPosition.Column < Position.Column ? -1 : 0);

            int currentRow = Position.Row + rowDirection;
            int currentColumn = Position.Column + colDirection;

            while (currentRow != newPosition.Row || currentColumn != newPosition.Column)
            {
                if (board.IsPositionOccupied(new Position(currentRow, currentColumn)))
                {
                    return false;
                }
                currentRow += rowDirection;
                currentColumn += colDirection;
            }

            var pieceAtNewPosition = board.GetPieceAtPosition(newPosition);
            if (pieceAtNewPosition == null || pieceAtNewPosition.Color != this.Color)
            {
                return true;
            }
        }
        return false;
    }

    public override List<Position> GetPossibleMoves(Board board)
    {
        var possibleMoves = new List<Position>();

        // Horizontal, vertical, and diagonal moves
        for (int i = 1; i < 8; i++)
        {
            var newPosition = new Position(Position.Row + i, Position.Column);
            if (IsMoveValid(newPosition, board))
            {
                possibleMoves.Add(newPosition);
            }

            newPosition = new Position(Position.Row - i, Position.Column);
            if (IsMoveValid(newPosition, board))
            {
                possibleMoves.Add(newPosition);
            }

            newPosition = new Position(Position.Row, Position.Column + i);
            if (IsMoveValid(newPosition, board))
            {
                possibleMoves.Add(newPosition);
            }

            newPosition = new Position(Position.Row, Position.Column - i);
            if (IsMoveValid(newPosition, board))
            {
                possibleMoves.Add(newPosition);
            }

            newPosition = new Position(Position.Row + i, Position.Column + i);
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

        if (rowDiff <= 1 && colDiff <= 1)
        {
            var pieceAtNewPosition = board.GetPieceAtPosition(newPosition);
            if (pieceAtNewPosition == null || pieceAtNewPosition.Color != this.Color)
            {
                // Temporarily move the King to the new position
                var originalPosition = Position;
                Position = newPosition;
                bool isInCheck = board.IsPositionUnderAttack(newPosition, this.Color);
                Position = originalPosition;

                // If the King is not in check in the new position, the move is valid
                if (!isInCheck)
                {
                    return true;
                }
            }
        }
        return false;
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

    public bool IsInCheck(Board board)
    {
        foreach (var piece in board.Pieces)
        {
            if (piece.Color != this.Color && piece.IsMoveValid(this.Position, board))
            {
                Console.WriteLine($"{this.Color} King is in check by {piece.Name} at {piece.Position}");
                return true;
            }
        }
        return false;
    }

    public bool IsInCheckmate(Board board)
    {
        if (!IsInCheck(board))
        {
            return false;
        }

        // Check if the king can move to any adjacent square
        foreach (var move in GetPossibleMoves(board))
        {
            var originalPosition = Position;
            Position = move;
            if (!IsInCheck(board))
            {
                Position = originalPosition;
                return false;
            }
            Position = originalPosition;
        }

        // Check if any other piece can make a valid move to get out of check
        foreach (var piece in board.Pieces)
        {
            if (piece.Color == Color)
            {
                foreach (var move in piece.GetPossibleMoves(board))
                {
                    var originalPosition = piece.Position;
                    piece.Position = move;
                    if (!IsInCheck(board))
                    {
                        piece.Position = originalPosition;
                        return false;
                    }
                    piece.Position = originalPosition;
                }
            }
        }

        return true;
    }

    public bool IsInStalemate(Board board)
    {
        if (IsInCheck(board))
        {
            return false;
        }

        foreach (var piece in board.Pieces)
        {
            if (piece.Color == Color && piece.GetPossibleMoves(board).Count > 0)
            {
                return false;
            }
        }

        return true;
    }
}
}