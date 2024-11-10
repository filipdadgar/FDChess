// Pieces.cs
using System.Text.Json.Serialization;
using System.Linq;

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
            int rowDifference = Math.Abs(newPosition.Row - Position.Row);
            int columnDifference = Math.Abs(newPosition.Column - Position.Column);

            if (rowDifference <= 1 && columnDifference <= 1 &&
                newPosition.Row >= 0 && newPosition.Row < 8 &&
                newPosition.Column >= 0 && newPosition.Column < 8)
            {
                var pieceAtNewPosition = board.GetPieceAtPosition(newPosition);
                if (pieceAtNewPosition == null || pieceAtNewPosition.Color != this.Color)
                {
                    return !IsPositionUnderAttack(newPosition, board);
                }
            }
            return false;
        }

        private bool IsPositionUnderAttack(Position position, Board board)
        {
            foreach (var piece in board.Pieces.Where(p => p.Color != this.Color && !p.IsRemoved))
            {
                if (piece.IsMoveValid(position, board))
                {
                    Console.WriteLine($"Position {position} is under attack by {piece.Color} {piece.Name} at {piece.Position}");
                    return true;
                }
            }
            return false;
        }

        public bool IsInCheck(Board board)
        {
            return IsPositionUnderAttack(Position, board);
        }

        public bool IsInCheckmate(Board board)
        {
            if (!IsInCheck(board))
                return false;

            Console.WriteLine($"Checking checkmate for {Color} king at {Position}");
            
            // Get all attacking pieces
            var attackingPieces = board.Pieces
                .Where(p => p.Color != Color && !p.IsRemoved && p.IsMoveValid(Position, board))
                .ToList();
            
            Console.WriteLine($"Found {attackingPieces.Count} attacking pieces");

            // Check all possible escape squares
            var escapeSquares = GetPossibleMoves(board);
            foreach (var escapeSquare in escapeSquares)
            {
                Console.WriteLine($"Checking escape square: {escapeSquare}");
                bool canEscape = true;
                foreach (var attacker in attackingPieces)
                {
                    if (attacker.IsMoveValid(escapeSquare, board))
                    {
                        Console.WriteLine($"Escape square {escapeSquare} is controlled by {attacker.Name} at {attacker.Position}");
                        canEscape = false;
                        break;
                    }
                }
                if (canEscape)
                {
                    Console.WriteLine($"King can escape to {escapeSquare}");
                    return false;
                }
            }

            // Check if any friendly piece can block or capture
            foreach (var defender in board.Pieces.Where(p => p.Color == Color && !p.IsRemoved))
            {
                foreach (var move in defender.GetPossibleMoves(board))
                {
                    var originalPosition = defender.Position;
                    var capturedPiece = board.GetPieceAtPosition(move);
                    bool wasRemoved = false;
                    
                    // Simulate the move
                    if (capturedPiece != null)
                    {
                        wasRemoved = capturedPiece.IsRemoved;
                        capturedPiece.IsRemoved = true;
                    }
                    defender.Position = move;
                    
                    bool stillInCheck = IsInCheck(board);
                    
                    // Restore the position
                    defender.Position = originalPosition;
                    if (capturedPiece != null)
                    {
                        capturedPiece.IsRemoved = wasRemoved;
                    }
                    
                    if (!stillInCheck)
                    {
                        Console.WriteLine($"Checkmate can be prevented by {defender.Name} moving to {move}");
                        return false;
                    }
                }
            }

            Console.WriteLine("Checkmate confirmed - no valid moves found");
            return true;
        }

        public override List<Position> GetPossibleMoves(Board board)
        {
            var possibleMoves = new List<Position>();
            var moveOffsets = new[]
            {
                (1, 0), (-1, 0), (0, 1), (0, -1),
                (1, 1), (1, -1), (-1, 1), (-1, -1)
            };

            foreach (var (rowOffset, columnOffset) in moveOffsets)
            {
                var newPosition = new Position(Position.Row + rowOffset, Position.Column + columnOffset);
                if (newPosition.Row >= 0 && newPosition.Row < 8 &&
                    newPosition.Column >= 0 && newPosition.Column < 8)
                {
                    if (IsMoveValid(newPosition, board))
                    {
                        possibleMoves.Add(newPosition);
                    }
                }
            }

            return possibleMoves;
        }

        public bool IsInStalemate(Board board)
        {
            if (IsInCheck(board))
                return false;

            Console.WriteLine($"Checking stalemate for {Color} king at {Position}");

            // Get all friendly pieces
            var friendlyPieces = board.Pieces
                .Where(p => p.Color == Color && !p.IsRemoved)
                .ToList();

            Console.WriteLine($"Found {friendlyPieces.Count} friendly pieces");

            // Check all possible moves for each friendly piece
            foreach (var friendlyPiece in friendlyPieces)
            {
                foreach (var move in friendlyPiece.GetPossibleMoves(board))
                {
                    var originalPosition = friendlyPiece.Position;
                    var capturedPiece = board.GetPieceAtPosition(move);
                    bool wasRemoved = false;

                    // Simulate the move
                    if (capturedPiece != null)
                    {
                        wasRemoved = capturedPiece.IsRemoved;
                        capturedPiece.IsRemoved = true;
                    }
                    friendlyPiece.Position = move;

                    bool stillInCheck = IsInCheck(board);

                    // Restore the position
                    friendlyPiece.Position = originalPosition;
                    if (capturedPiece != null)
                    {
                        capturedPiece.IsRemoved = wasRemoved;
                    }

                    if (!stillInCheck)
                    {
                        Console.WriteLine($"Stalemate can be prevented by {friendlyPiece.Name} moving to {move}");
                        return false;
                    }
                }
            }

            Console.WriteLine("Stalemate confirmed - no valid moves found");
            return true;
        }
    }
}
