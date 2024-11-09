using System.Text.Json.Serialization;

namespace FDChess.Model
{
    /// <summary>
    /// Chess Game Class
    /// </summary>
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public Board Board { get; set; }
        public string CurrentTurn { get; set; } = "white"; // Initialize with white's turn
        public string GameStatus { get; set; } = "active"; // Initialize with active status

        [JsonConstructor]
        public Game()
        {
            Board = new Board();
        }

        public Game(int id, string name, string state, Board board)
        {
            Id = id;
            Name = name;
            State = state;
            Board = board;
        }

        public override string ToString()
        {
            return $"{Name} - {State}";
        }
        
        public void ChangeTurn()
        {
            CurrentTurn = CurrentTurn == "white" ? "black" : "white";
        }
        
        public void EndGame()
        {
            GameStatus = "ended";
        }
        
        // Return list of removed pieces
        public List<Piece> RemovePieces(List<Position> positions)
        {
            var removedPieces = new List<Piece>();
            foreach (var position in positions)
            {
                var piece = Board.GetPieceAtPosition(position);
                if (piece != null)
                {
                    removedPieces.Add(piece);
                    Board.RemovePiece(position);
                }
            }
            return removedPieces;
        }
        
        // return list of pieces currently on the board
        public List<Piece> GetPieces()
        {
            return Board.Pieces;
        }
    }
}