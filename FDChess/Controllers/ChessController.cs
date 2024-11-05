using FDChess.Interfaces;
using FDChess.Services;
using FDChess.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            if (request == null || request.Board == null || request.NewPosition.Equals(default(Position)))
            {
                return BadRequest("Invalid move request.");
            }

            try
            {
                var result = _chessService.MakeMove(request.Board, request.NewPosition);
                var response = new { message = result };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new { error = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
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
                        Pieces = InitializeDefaultPieces()
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

        private List<Piece?> InitializeDefaultPieces()
        {
            // Initialize the board with default pieces
            var pieces = new List<Piece?>
            {
                // Add pieces for both players
                // Example: new Rook(1, "Rook", new Position(0, 0), "white"),
                // Add other pieces similarly
            };
            return pieces;
        }
    }

    public class MoveRequest
    {
        public required Board Board { get; set; }
        public required Position NewPosition { get; set; }
    }
}
