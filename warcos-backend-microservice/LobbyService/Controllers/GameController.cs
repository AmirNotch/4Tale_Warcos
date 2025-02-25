using Lobby.Models.Matchmaking;
using Lobby.Service;
using Lobby.Controllers;
using Lobby.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Lobby.Controllers;

[ApiController]
[Route("api/private")]
public class GameController : BaseController
{
    private readonly GameService _gameService;
    private readonly GameRegimeService _gameRegimeService;
    private readonly StartGameService _startGameService;
    public GameController(GameService gameService, GameRegimeService gameRegimeService,
        ValidationStorage validationStorage, StartGameService startGameService) : base(validationStorage)
    {
        _gameService = gameService;
        _gameRegimeService = gameRegimeService;
        _startGameService = startGameService;
    }

    [HttpPost("startGame")]
    public async Task<IActionResult> StartGame([FromBody] StartGameRequest request, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _startGameService.StartGame(request, token), ct);
    }

    [HttpPost("finishGame")]
    public async Task<IActionResult> FinishGame([FromBody] Guid gameId, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _gameService.DoFinishGame(gameId, token), ct);
    }

    [HttpGet("getCurrentGameRegime")]
    public async Task<IActionResult> GetCurrentGameRegime([FromQuery] string gameMode, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _gameRegimeService.GetCurrentGameRegime(gameMode, token), ct);
    }

    [HttpGet("getGameModes")]
    public async Task<IActionResult> GetGameModes(CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _gameRegimeService.GetAllGameModes(token), ct);
    }

    [HttpGet("getConnectedUsers")]
    public ActionResult GetConnectedUsers(CancellationToken ct)
    {
        return Ok(WebSocketConnectionManager.GetConnectedUserIds());
    }
}