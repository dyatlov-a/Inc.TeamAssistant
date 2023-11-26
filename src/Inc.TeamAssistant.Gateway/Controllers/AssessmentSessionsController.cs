using Inc.TeamAssistant.Appraiser.Model;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

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
	public async Task<IActionResult> GetStoryDetails(Guid assessmentSessionId, CancellationToken cancellationToken)
		=> Ok(await _service.GetStoryDetails(assessmentSessionId, cancellationToken));

    [HttpGet("link-for-connect")]
    public async Task<IActionResult> LinkForConnect(CancellationToken cancellationToken)
        => Ok(await _service.GetLinkForConnect(cancellationToken));
}