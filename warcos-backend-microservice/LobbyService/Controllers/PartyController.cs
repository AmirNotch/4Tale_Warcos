using Lobby.Service;
using Lobby.Controllers;
using Lobby.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Lobby.Controllers;

[ApiController]
[Route("api/private")]
public class PartyController : BaseController
{
    private readonly ILogger<PartyController> _logger;
    private readonly PartyService _partyService;
    public PartyController(ILogger<PartyController> logger, PartyService partyService, IValidationStorage validationStorage) : base(validationStorage)
    {
        _logger = logger;
        _partyService = partyService;
    }

    [HttpPost("cancelGameSearchByTicketIds")]
    public async Task<IActionResult> CancelGameSearchByTicketIds([FromBody] List<string> ticketIds, CancellationToken ct)
    {
        return await HandleRequestAsync(async token => await _partyService.CancelGameSearchByTicketIds(ticketIds, token), ct);
    }
}
