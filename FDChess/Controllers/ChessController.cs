using FDChess.Interfaces;
using FDChess.Services;
using FDChess.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FDChess.Helper;

namespace FDChess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChessController : ControllerBase
    {
        private readonly IChessService _chessService;

        public ChessController(IChessService chessService)
        {
            _chessService = chessService;
        }
        
        // ChessController.cs
        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] MoveRequest moveRequest)
        {
            try
            {
                var gameState = _chessService.GetGameState();
                var options = new JsonSerializerOptions { Converters = { new PieceConverter() } };
                var game = JsonSerializer.Deserialize<Game>(gameState, options);
                var board = game.Board;

                board.MovePiece(moveRequest.CurrentPosition, moveRequest.NewPosition);

                // Save updated game state
                _chessService.SetGameState(JsonSerializer.Serialize(game, options));
                return Ok(game); // Return the updated game state
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("state")]
        public IActionResult GetGameState()
        {
            try
            {
                var state = _chessService.GetGameState();
                if (string.IsNullOrEmpty(state))
                {
                    // Initialize game state if it is null or empty
                    var defaultBoard = new Board
                    {
                        Id = 1,
                        Name = "Default Board",
                        Description = "Initial game state",
                        Pieces = _chessService.InitializeDefaultPieces()
                    };
                    var defaultGame = new Game
                    {
                        Id = 1,
                        Name = "Default Game",
                        State = "active",
                        Board = defaultBoard
                    };
                    _chessService.SetGameState(JsonSerializer.Serialize(defaultGame));
                    state = _chessService.GetGameState();
                }
                return Ok(state);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpPost("reset")]
        public IActionResult ResetGame()
        {
            try
            {
                _chessService.ResetGame();
                return Ok(new { message = "Game has been reset" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // ChessController.cs
        [HttpGet("moves/{pieceId}")]
        public IActionResult GetPossibleMoves(int pieceId)
        {
            try
            {
                var possibleMoves = _chessService.GetPossibleMoves(pieceId);
                return Ok(possibleMoves);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

    public class MoveRequest
    {
        public Position CurrentPosition { get; set; }
        public Position NewPosition { get; set; }
    }
}