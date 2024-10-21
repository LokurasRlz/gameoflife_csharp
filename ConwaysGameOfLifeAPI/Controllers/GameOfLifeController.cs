using ConwaysGameOfLifeAPI.Models;
using ConwaysGameOfLifeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConwaysGameOfLifeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameOfLifeController : ControllerBase
    {
        private readonly IGameOfLifeService _gameOfLifeService;

        public GameOfLifeController(IGameOfLifeService gameOfLifeService)
        {
            _gameOfLifeService = gameOfLifeService;
        }

        [HttpPost("upload")]
        public ActionResult<Guid> UploadBoard(int rows, int columns, [FromBody] int[][] initialState)
        {
            var boardId = _gameOfLifeService.UploadBoard(rows, columns, initialState);
            return Ok(boardId);
        }

        [HttpGet("{boardId}/next")]
        public ActionResult<GameResult> GetNextState(Guid boardId)
        {
            var result = _gameOfLifeService.GetNextState(boardId);
            return Ok(result);
        }

        [HttpGet("{boardId}/nth/{n}")]
        public ActionResult<GameResult> GetNthState(Guid boardId, int n)
        {
            var result = _gameOfLifeService.GetNthState(boardId, n);
            return Ok(result);
        }

        [HttpGet("{boardId}/final/{maxGenerations}")]
        public ActionResult<GameResult> GetFinalState(Guid boardId, int maxGenerations)
        {
            var result = _gameOfLifeService.GetFinalState(boardId, maxGenerations);
            return Ok(result);
        }
    }
}
