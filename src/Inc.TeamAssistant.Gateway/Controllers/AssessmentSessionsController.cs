using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

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
    public async Task<IActionResult> GetActiveStory(Guid teamId, CancellationToken token)
    {
        return Ok(await _service.GetActiveStory(teamId, token));
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(Guid teamId, DateOnly? from, CancellationToken token)
    {
        return Ok(await _service.GetAssessmentHistory(teamId, from, token));
    }
    
    [HttpGet("stories/{teamId}/{assessmentDate}")]
    public async Task<IActionResult> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token)
    {
        return Ok(await _service.GetStories(teamId, assessmentDate, token));
    }
}