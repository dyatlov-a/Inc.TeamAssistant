using Inc.TeamAssistant.Appraiser.Model;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("sessions")]
public sealed class SessionsController : ControllerBase
{
	private readonly IAppraiserService _service;

    public SessionsController(IAppraiserService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet("story/{teamId}")]
	public async Task<IActionResult> GetStoryDetails(Guid teamId, CancellationToken cancellationToken)
		=> Ok(await _service.GetStoryDetails(teamId, cancellationToken));

    [HttpGet("link-for-connect")]
    public async Task<IActionResult> LinkForConnect(CancellationToken cancellationToken)
        => Ok(await _service.GetLinkForConnect(cancellationToken));
}