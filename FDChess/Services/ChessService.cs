using System.Text.Json;
using FDChess.Controllers;
using FDChess.Helper;
using FDChess.Interfaces;
using FDChess.Model;
using Microsoft.SemanticKernel.Services;

namespace FDChess.Services
{
    /// <summary>
    /// Chess Service Class
    /// </summary>
    public class ChessService : IChessService
    {
        // Variables
        private Game _currentGame;
        private readonly JsonSerializerOptions _options;
        private readonly AIService _aiService;


        public ChessService()
        {
            _options = new JsonSerializerOptions { Converters = { new PieceConverter() } };

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

        public ChessService(AIService aiService)
        {
            _options = new JsonSerializerOptions { Converters = { new PieceConverter() } };
            _aiService = aiService;

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

        public List<Piece> InitializeDefaultPieces()
        {
            return new List<Piece>
            {
                new Rook(1, new Position(0, 0), "white"),
                new Knight(2, new Position(0, 1), "white"),
                new Bishop(3, new Position(0, 2), "white"),
                new Queen(4, new Position(0, 3), "white"),
                new King(5, new Position(0, 4), "white"),
                new Bishop(6, new Position(0, 5), "white"),
                new Knight(7, new Position(0, 6), "white"),
                new Rook(8, new Position(0, 7), "white"),
                new Pawn(9, new Position(1, 0), "white"),
                new Pawn(10, new Position(1, 1), "white"),
                new Pawn(11, new Position(1, 2), "white"),
                new Pawn(12, new Position(1, 3), "white"),
                new Pawn(13, new Position(1, 4), "white"),
                new Pawn(14, new Position(1, 5), "white"),
                new Pawn(15, new Position(1, 6), "white"),
                new Pawn(16, new Position(1, 7), "white"),
                new Rook(17, new Position(7, 0), "black"),
                new Knight(18, new Position(7, 1), "black"),
                new Bishop(19, new Position(7, 2), "black"),
                new Queen(20, new Position(7, 3), "black"),
                new King(21, new Position(7, 4), "black"),
                new Bishop(22, new Position(7, 5), "black"),
                new Knight(23, new Position(7, 6), "black"),
                new Rook(24, new Position(7, 7), "black"),
                new Pawn(25, new Position(6, 0), "black"),
                new Pawn(26, new Position(6, 1), "black"),
                new Pawn(27, new Position(6, 2), "black"),
                new Pawn(28, new Position(6, 3), "black"),
                new Pawn(29, new Position(6, 4), "black"),
                new Pawn(30, new Position(6, 5), "black"),
                new Pawn(31, new Position(6, 6), "black"),
                new Pawn(32, new Position(6, 7), "black")
            };
        }
        
        public string MakeMove(MoveRequest moveRequest)
        {
            var piece = _currentGame.Board.GetPieceAtPosition(moveRequest.CurrentPosition);
            if (piece == null)
            {
                return JsonSerializer.Serialize(new { message = "Invalid move" });
            }

            if (piece.Color != _currentGame.CurrentTurn)
            {
                return JsonSerializer.Serialize(new { message = "Not your turn" });
            }

            // Check if the move would put the king in check
            var kingPosition = _currentGame.Board.Pieces
                .FirstOrDefault(p => p is King && p.Color == piece.Color)?.Position;
            if (kingPosition != null)
            {
                var tempBoard = CloneBoard(_currentGame.Board);
                tempBoard.MovePiece(piece.Position, moveRequest.NewPosition);
                var kingPiece = tempBoard.Pieces.FirstOrDefault(p => p is King && p.Color == piece.Color) as King;
                if (kingPiece != null && kingPiece.IsInCheck(tempBoard))
                {
                    return JsonSerializer.Serialize(new { message = "Invalid move: This move would put your king in check" });
                }
            }

            if (piece is King king && king.IsPositionUnderAttack(moveRequest.NewPosition, _currentGame.Board))
            {
                return JsonSerializer.Serialize(new { message = "Invalid move: The king cannot move to a position where it would be in check" });
            }

            try
            {
                _currentGame.Board.MovePiece(piece.Position, moveRequest.NewPosition);
                var opponentColor = piece.Color == "white" ? "black" : "white";

                // Check for pawn promotion
                if (piece is Pawn && (moveRequest.NewPosition.Row == 0 || moveRequest.NewPosition.Row == 7))
                {
                    return JsonSerializer.Serialize(new { message = "Pawn promotion required", gameState = _currentGame });
                }
                
                // Check for check, checkmate, and stalemate conditions
                if (_currentGame.Board.IsKingInCheck(opponentColor))
                {
                    Console.WriteLine($"King in check: {opponentColor}");
                    // Check for checkmate before switching turns
                    if (_currentGame.Board.IsKingInCheckmate(opponentColor))
                    {
                        Console.WriteLine($"Checkmate detected for {opponentColor}");
                        _currentGame.GameStatus = "checkmate";
                        _currentGame.CurrentTurn = "none"; // Game is over
                        Console.WriteLine("Game over");
                        return JsonSerializer.Serialize(new { message = "Checkmate", gameState = _currentGame });
                    }
                    
                    Console.WriteLine($"Check detected for {opponentColor}");
                    _currentGame.CurrentTurn = opponentColor;
                    return JsonSerializer.Serialize(new { message = "Check", gameState = _currentGame });
                }
                if (_currentGame.Board.IsKingInStalemate(opponentColor))
                {
                    _currentGame.GameStatus = "stalemate";
                    return JsonSerializer.Serialize(new { message = "Stalemate", gameState = _currentGame });
                }

                // Switch turns
                _currentGame.CurrentTurn = opponentColor;
                _currentGame.GameStatus = "active";
            }
            catch (InvalidOperationException ex)
            {
                return JsonSerializer.Serialize(new { message = ex.Message });
            }

            // Update the current game state
            return JsonSerializer.Serialize(new { message = "Move successful", gameState = _currentGame });
        }
        
        public string GetGameState()
        {
            return JsonSerializer.Serialize(_currentGame, _options);
        }

        public void SetGameState(string gameState)
        {
            _currentGame = JsonSerializer.Deserialize<Game>(gameState, _options)!;
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
                case "queen":
                    return new Queen(id, position, color);
                case "rook":
                    return new Rook(id, position, color);
                case "bishop":
                    return new Bishop(id, position, color);
                case "knight":
                    return new Knight(id, position, color);
                default:
                    throw new ArgumentException("Invalid piece name");
            }
        }
        
        public List<Position> GetPossibleMoves(int pieceId)
        {
            var piece = _currentGame.Board.Pieces.FirstOrDefault(p => p.Id == pieceId);
            if (piece == null)
            {
                throw new InvalidOperationException("Piece not found");
            }

            // Assuming each piece has a method to get possible moves
            return piece.GetPossibleMoves(_currentGame.Board);
        }
        
        public void ResetGame()
        {
            var board = new Board
            {
                Id = 1,
                Name = "Default Game",
                Description = "Initial game state",
                Pieces = InitializeDefaultPieces()
            };
            _currentGame = new Game(1, "Default Game", "active", board);
        }
        
        // Return removed pieces from the board
        public List<Piece> GetRemovedPieces()
        {
            return _currentGame.Board.Pieces.Where(p => p.IsRemoved).ToList();
        }
        
        // Get available pieces on the board
        public List<Piece> GetAvailablePieces()
        {
            return _currentGame.Board.Pieces.Where(p => !p.IsRemoved).ToList();
        }

        private Board CloneBoard(Board originalBoard)
        {
            var serialized = JsonSerializer.Serialize(originalBoard, _options);
            return JsonSerializer.Deserialize<Board>(serialized, _options)!;
        }
        
        public string PromotePawn(Position position, string newPieceType)
        {
            var pawn = _currentGame.Board.GetPieceAtPosition(position) as Pawn;
            if (pawn == null)
            {
                return JsonSerializer.Serialize(new { message = "Invalid promotion: No pawn at the given position" });
            }

            if ((pawn.Color == "white" && position.Row != 7) || (pawn.Color == "black" && position.Row != 0))
            {
                return JsonSerializer.Serialize(new { message = "Invalid promotion: Pawn is not at the last rank" });
            }

            if (pawn.Color != null)
            {
                Piece? newPiece = CreatePiece(newPieceType, pawn.Id, position, pawn.Color);
                if (newPiece == null)
                {
                    return JsonSerializer.Serialize(new { message = "Invalid promotion: Invalid piece type" });
                }

                _currentGame.Board.Pieces.Remove(pawn);
                _currentGame.Board.Pieces.Add(newPiece);
            }

            return JsonSerializer.Serialize(new { message = "Pawn promoted", gameState = _currentGame });
        }

        public async Task<string> DescribeBoardAsync()
        {
            if (_aiService == null)
            {
                throw new InvalidOperationException("_aiService is not initialized.");
            }

            if (_currentGame?.Board == null)
            {
                throw new InvalidOperationException("_currentGame or _currentGame.Board is not initialized.");
            }

            return await _aiService.DescribeBoardAsync(_currentGame.Board);
        }

    }
}
