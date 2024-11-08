﻿using System.Text.Json;
using FDChess.Controllers;
using FDChess.Helper;
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
        private readonly JsonSerializerOptions _options;
        
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

        private bool ValidateMove()
        {
            // Implement move validation logic
            return true; // Placeholder return value
        }
        
        public string MakeMove(MoveRequest moveRequest)
        {
            var piece = _currentGame.Board.GetPieceAtPosition(moveRequest.CurrentPosition);
            if (piece == null)
            {
                return JsonSerializer.Serialize(new { message = "Invalid move" });
            }

            try
            {
                _currentGame.Board.MovePiece(piece.Position, moveRequest.NewPosition);
                var opponentColor = piece.Color == "white" ? "black" : "white";
                if (_currentGame.Board.IsKingInCheck(opponentColor))
                {
                    return JsonSerializer.Serialize(new { message = "Check", gameState = _currentGame });
                }
                if (_currentGame.Board.IsKingInCheckmate(opponentColor))
                {
                    return JsonSerializer.Serialize(new { message = "Checkmate", gameState = _currentGame });
                }
                if (_currentGame.Board.IsKingInStalemate(opponentColor))
                {
                    return JsonSerializer.Serialize(new { message = "Stalemate", gameState = _currentGame });
                }
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
    }
}
