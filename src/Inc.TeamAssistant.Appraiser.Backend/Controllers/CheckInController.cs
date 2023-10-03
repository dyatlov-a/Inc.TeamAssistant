using Inc.TeamAssistant.CheckIn.Model;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Appraiser.Backend.Controllers;

[ApiController]
[Route("check-in")]
public sealed class CheckInController : ControllerBase
{
    private readonly ICheckInService _service;

    public CheckInController(ICheckInService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet("{mapId}")]
    public async Task<IActionResult> GetStoryDetails(Guid mapId, CancellationToken cancellationToken)
        => Ok(await _service.GetLocations(mapId, cancellationToken));
}