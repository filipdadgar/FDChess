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
        
        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] MoveRequest moveRequest)
        {
            try
            {
                var result = _chessService.MakeMove(moveRequest);
                var response = JsonSerializer.Deserialize<Dictionary<string, object>>(result);

                if (response != null && response.ContainsKey("message"))
                {
                    var message = response["message"].ToString();
                    var gameState = response.ContainsKey("gameState") ? response["gameState"] : null;

                    if (message == "Check" || message == "Checkmate" || message == "Stalemate")
                    {
                        return Ok(new { message, gameState });
                    }
                    else if (message == "Invalid move")
                    {
                        return BadRequest(new { message });
                    }
                }

                return Ok(result);
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
        
        [HttpPost("simulate")]
        public IActionResult SimulateGame()
        {
            try
            {
                var simulation = new QuickGameSimulation();
                simulation.Run();
                var state = _chessService.GetGameState();
                return Ok(state);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // Get list of pieces on the board with color and position
        [HttpGet("pieces")]
        public IActionResult GetPieces()
        {
            try
            {
                var state = _chessService.GetGameState();
                var game = JsonSerializer.Deserialize<Game>(state);
                var pieces = game.GetPieces();
                return Ok(pieces);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        // Get list of removed pieces
        [HttpGet("removed")]
        public IActionResult GetRemovedPieces([FromQuery] List<Position> positions)
        {
            try
            {
                var state = _chessService.GetGameState();
                var game = JsonSerializer.Deserialize<Game>(state);
                if (game != null)
                {
                    var removedPieces = game.RemovePieces(positions);
                    return Ok(removedPieces);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(new List<Piece>());
        }
    }

    public class MoveRequest
    {
        public Position CurrentPosition { get; set; }
        public Position NewPosition { get; set; }
    }
}