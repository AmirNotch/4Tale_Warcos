using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Lobby.Service;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Lobby.Controllers;

[ApiController]
[Route("api/public")]
public class PlayController : ControllerBase
{
    private readonly ILogger<PlayController> _logger;
    private readonly WebSocketHandler _userWebSocketService;
    private readonly UserService _userService;

    public PlayController(ILogger<PlayController> logger, WebSocketHandler userWebSocketService, UserService userService)
    {
        _logger = logger;
        _userWebSocketService = userWebSocketService;
        _userService = userService;
    }

    [Authorize]
    [Route("ws")]
    [HttpGet]
    public async Task ConnectByWebSocket(CancellationToken ct)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        Guid? userIdOptional;
        string? error;
        string? profile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        _logger.LogDebug("Got connection request. Profile = {profile}", profile);
        if (profile == "Development" || profile == "Testing")
        {
            (userIdOptional, error) = getUserIdFromClaimsStub();
        }
        else
        {
            (userIdOptional, error) = getUserIdFromClaims();
        }
        if (error != null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(error));
            _logger.LogWarning(error);
            return;
        }
        _logger.LogInformation("User {userId} requested a connection", userIdOptional);

        Guid userId = userIdOptional!.Value;
        await _userService.CreateIfAbsent(userId, ct);

        using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await _userWebSocketService.HandleWebSocket(ws, userId, ct);
    }

    private (Guid?, string?) getUserIdFromClaims()
    {
        var identity = (ClaimsIdentity)User.Identity!;
        IEnumerable<Claim> claims = identity.Claims;
        Claim? userIdClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return (Guid.Empty, "Cannot find userId claim!");
        }
        bool parseResult = Guid.TryParse(userIdClaim.Value, out Guid leaderUserId);
        if (!parseResult)
        {
            return (Guid.Empty, "Cannot begin game search: cannot parse leader user id!");
        }
        return (leaderUserId, null);
    }

    private (Guid?, string?) getUserIdFromClaimsStub()
    {
        if (HttpContext.Request.Headers.ContainsKey("Id"))
        {
            StringValues header = HttpContext.Request.Headers["Id"];
            return (Guid.Parse(header[0]), null);
        }
        else
        {
            return (null, "Missing Id header");
        }
    }
}
