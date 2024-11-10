using System.Collections.Generic;
using System.Linq;
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
            Pieces = new List<Piece>();
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

        public bool IsPositionOccupiedByOpponent(Position position, string color)
        {
            var piece = GetPieceAtPosition(position);
            return piece != null && piece.Color != color;
        }

        public void RemovePiece(Position position, Piece removingPiece)
        {
            var piece = GetPieceAtPosition(position);
            if (piece != null)
            {
                if (piece is King)
                {
                    throw new InvalidOperationException("The king cannot be removed from the board.");
                }
                piece.IsRemoved = true;
                piece.Position = new Position(-1, -1); // Set to an invalid position
                piece.RemovedBy = removingPiece.Color + " " + removingPiece.Name; // Track the piece that removed it
                piece.RemovedAt = position; // Track the position where it was removed
            }
        }

        public void MovePiece(Position from, Position to)
        {
            var piece = GetPieceAtPosition(from);
            if (piece == null) throw new InvalidOperationException("No piece at the starting position.");
            if (!piece.IsMoveValid(to, this)) throw new InvalidOperationException("Invalid move for the piece.");
            if (piece.Color != null && IsPositionOccupiedByOpponent(to, piece.Color))
            {
                RemovePiece(to, piece);
            }
            piece.Position = to;
        }

        public bool IsKingInCheck(string color)
        {
            var king = Pieces.OfType<King>().FirstOrDefault(k => k.Color == color);
            if (king == null)
            {
                throw new InvalidOperationException("King not found on the board.");
            }
            Console.WriteLine($"IsKingInCheck for {color}: {king.IsInCheck(this)}");
            return king.IsInCheck(this);
        }

        public bool IsKingInCheckmate(string color)
        {
            var king = Pieces.OfType<King>().FirstOrDefault(k => k.Color == color);
            if (king == null)
            {
                throw new InvalidOperationException("King not found on the board.");
            }
            bool isInCheckmate = king.IsInCheckmate(this);
            Console.WriteLine($"IsKingInCheckmate for {color}: {isInCheckmate}");
            return isInCheckmate;
        }

        public bool IsKingInStalemate(string color)
        {
            var king = Pieces.OfType<King>().FirstOrDefault(k => k.Color == color);
            if (king == null)
            {
                throw new InvalidOperationException("King not found on the board.");
            }
            return king.IsInStalemate(this);
        }
        
        public bool IsPositionUnderAttack(Position position, string color)
        {
            return Pieces.Any(p => p.Color != color && p.IsMoveValid(position, this));
        }
    }
}