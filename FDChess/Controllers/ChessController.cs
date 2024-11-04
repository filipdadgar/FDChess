using FDChess.Services;
using FDChess.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace FDChess.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class ChessController : ControllerBase
    {
        private readonly ChessService _chessService;

        public ChessController(ChessService chessService)
        {
            _chessService = chessService;
        }

        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] Board request)
        {
            var result = _chessService.MakeMove(request);
            return Ok(result);
        }

        [HttpGet("state")]
        public IActionResult GetGameState()
        {
            var state = _chessService.GetGameState();
            return Ok(state);
        }
    }
}
