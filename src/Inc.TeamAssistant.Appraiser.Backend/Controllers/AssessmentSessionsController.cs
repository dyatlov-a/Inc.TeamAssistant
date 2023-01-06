using Inc.TeamAssistant.Appraiser.Model;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Appraiser.Backend.Controllers;

[ApiController]
[Route("sessions")]
public sealed class AssessmentSessionsController : ControllerBase
{
	private readonly IAssessmentSessionsService _service;

    public AssessmentSessionsController(IAssessmentSessionsService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet("story/{assessmentSessionId}")]
	public async Task<IActionResult> GetStoryDetails(
        Guid assessmentSessionId,
        int width,
        int height,
        bool drawQuietZones,
        CancellationToken cancellationToken)
		=> Ok(await _service.GetStoryDetails(new(assessmentSessionId), width, height, drawQuietZones, cancellationToken));

    [HttpGet("link-for-connect")]
    public async Task<IActionResult> LinkForConnect(
        int width,
        int height,
        bool drawQuietZones,
        CancellationToken cancellationToken)
        => Ok(await _service.GetLinkForConnect(width, height, drawQuietZones, cancellationToken));
}