using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("assessment-sessions")]
public sealed class AssessmentSessionsController : ControllerBase
{
	private readonly IAppraiserService _service;

    public AssessmentSessionsController(IAppraiserService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    [HttpGet("story/{teamId}/active")]
    public async Task<IActionResult> GetActiveStory(
        Guid teamId,
        [FromQuery]string foreground,
        [FromQuery]string background,
        CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);
        
        return Ok(await _service.GetActiveStory(teamId, foreground, background, token));
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(Guid teamId, int? limit, DateOnly? from, CancellationToken token)
    {
        return Ok(await _service.GetAssessmentHistory(teamId, limit, from, token));
    }
    
    [HttpGet("stories/{teamId}/{assessmentDate}")]
    public async Task<IActionResult> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token)
    {
        return Ok(await _service.GetStories(teamId, assessmentDate, token));
    }
}