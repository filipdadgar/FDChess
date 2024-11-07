using System.Collections.Generic;
using System.Text.Json.Serialization;
using FDChess.Helper;

namespace FDChess.Model
{
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(PieceListConverter))]
        public List<Piece> Pieces { get; set; } = new List<Piece>();

        public Board()
        {
            Pieces = new List<Piece?>();
        }

        [JsonConstructor]
        public Board(int id) : this()
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"{Name} - {Description}";
        }

        public Piece? GetPieceAtPosition(Position position)
        {
            return Pieces.FirstOrDefault(p => p?.Position.Equals(position) == true);
        }

        public bool IsPositionOccupied(Position position)
        {
            return GetPieceAtPosition(position) != null;
        }

        public bool IsPathClear(Position start, Position end)
        {
            var startRow = start.Row;
            var startCol = start.Column;
            var endRow = end.Row;
            var endCol = end.Column;

            var rowDirection = Math.Sign(endRow - startRow);
            var colDirection = Math.Sign(endCol - startCol);

            var currentRow = startRow + rowDirection;
            var currentCol = startCol + colDirection;

            while (currentRow != endRow || currentCol != endCol)
            {
                var position = new Position(currentRow, currentCol);
                if (IsPositionOccupied(position)) return false;

                currentRow += rowDirection;
                currentCol += colDirection;
            }
            return true;
        }

        public void AddPiece(Piece piece)
        {
            if (piece == null) throw new ArgumentNullException(nameof(piece));
            if (IsPositionOccupied(piece.Position))
                throw new InvalidOperationException("Position is already occupied by another piece.");
            Pieces.Add(piece);
        }

        public void RemovePiece(Position position)
        {
            var piece = GetPieceAtPosition(position);
            if (piece != null)
            {
                Pieces.Remove(piece);
            }
        }

        public void MovePiece(Position from, Position to)
        {
            var piece = GetPieceAtPosition(from);
            if (piece == null) throw new InvalidOperationException("No piece at the starting position.");
            if (!piece.IsMoveValid(to, this)) throw new InvalidOperationException("Invalid move for the piece.");
            if (IsPositionOccupied(to)) throw new InvalidOperationException("Position is already occupied by another piece.");

            piece.Position = to;
        }
    }
}