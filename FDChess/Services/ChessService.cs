using System.Text.Json;
using FDChess.Interfaces;
using FDChess.Model;

namespace FDChess.Services
{
    /// <summary>
    /// Chess Service Class
    /// </summary>
    public class ChessService : IChessService
    {
        // Variables
        private Game _currentGame;

        public ChessService()
        {
            // Initialize the game with a default state
            var board = new Board
            {
                Id = 1,
                Name = "Default Game",
                Description = "Initial game state",
                Pieces = InitializeDefaultPieces()
            };
            _currentGame = new Game(1, "Default Game", "active", board);
        }

        private List<Piece> InitializeDefaultPieces()
        {
            // Initialize the board with default pieces
            return new List<Piece>
            {
                new Rook(1, new Position(0, 0), "white"),
                new Knight(2, new Position(0, 1), "white"),
                // Add other pieces...
                new Pawn(9, new Position(1, 0), "white"),
                // Add other pawns...
                new Rook(17, new Position(7, 0), "black"),
                new Knight(18, new Position(7, 1), "black"),
                // Add other pieces...
                new Pawn(25, new Position(6, 0), "black"),
                // Add other pawns...
            };
        }


        private bool ValidateMove(Board board)
        {
            // Implement move validation logic
            return true; // Placeholder return value
        }

        public string MakeMove(Board request, Position newPosition)
        {
            // Validate the move (this is a placeholder, implement actual validation logic)
            bool isValidMove = ValidateMove(request);
            if (!isValidMove)
            {
                return JsonSerializer.Serialize(new { message = "Invalid move" });
            }

            // Update the board state (this is a placeholder, implement actual update logic)
            UpdateBoardState(request);

            // Check for special conditions (e.g., check, checkmate)
            string gameState = CheckGameState(request);

            // Update the current game state
            _currentGame.Board = request;
            _currentGame.State = gameState;

            // Return the new game state or a confirmation message
            return JsonSerializer.Serialize(new { message = gameState });
        }


        public string GetGameState()
        {
            return JsonSerializer.Serialize(_currentGame);
        }

        public void SetGameState(string gameState)
        {
            _currentGame = JsonSerializer.Deserialize<Game>(gameState)!;
        }

        public Game CreateGame(string name, string state, Board board)
        {
            _currentGame = new Game(1, name, state, board);
            return _currentGame;
        }

        public Piece? AddPieceToBoard(Board board, string name, Position position, string color)
        {
            // Assuming there are concrete implementations of Piece, e.g., Pawn, Rook, etc.
            Piece? piece = CreatePiece(name, board.Pieces.Count + 1, position, color);
            board.Pieces.Add(piece);
            return piece;
        }

        private Piece? CreatePiece(string name, int id, Position position, string color)
        {
            // Implement logic to create specific piece types based on the name
            switch (name.ToLower())
            {
                case "pawn":
                    return new Pawn(id, position, color);
                case "rook":
                    return new Rook(id, position, color);
                // Add cases for other piece types
                default:
                    throw new ArgumentException("Invalid piece name");
            }
        }

        public List<Piece?> GetPieces(Board board)
        {
            return board.Pieces;
        }

        private void UpdateBoardState(Board board)
        {
            // Implement board state update logic
        }

        private string CheckGameState(Board board)
        {
            // Implement game state checking logic
            return "Game in progress";
        }
    }
}
