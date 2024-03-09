using Inc.TeamAssistant.Appraiser.Model;
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
    
    [HttpGet("link-for-connect")]
    public async Task<IActionResult> LinkForConnect(CancellationToken token)
    {
        return Ok(await _service.GetLinkForConnect(token));
    }

    [HttpGet("story/{teamId}/current")]
    public async Task<IActionResult> GetStoryDetails(Guid teamId, CancellationToken token)
    {
        return Ok(await _service.GetStoryDetails(teamId, token));
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(Guid teamId, int depth, CancellationToken token)
    {
        return Ok(await _service.GetAssessmentHistory(teamId, depth, token));
    }
    
    [HttpGet("stories/{teamId}/{assessmentDate}")]
    public async Task<IActionResult> GetStories(Guid teamId, DateOnly assessmentDate, CancellationToken token)
    {
        return Ok(await _service.GetStories(teamId, assessmentDate, token));
    }
    
    [HttpGet("story/{storyId}")]
    public async Task<IActionResult> GetStoryById(Guid storyId, CancellationToken token)
    {
        return Ok(await _service.GetStoryById(storyId, token));
    }
}