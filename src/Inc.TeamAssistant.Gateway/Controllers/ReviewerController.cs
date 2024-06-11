using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Controllers;

[ApiController]
[Route("reviewer")]
[Authorize]
public sealed class ReviewerController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewerController(IReviewService reviewService)
    {
        _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
    }
    
    [HttpGet("history/{teamId:guid}/{depth:int}")]
    public async Task<IActionResult> GetHistory(Guid teamId, int depth)
    {
        var result = await _reviewService.GetHistory(teamId, depth);
        return Ok(result);
    }

    [HttpGet("average/{teamId:guid}/{depth:int}")]
    public async Task<IActionResult> GetAverage(Guid teamId, int depth)
    {
        var result = await _reviewService.GetAverage(teamId, depth);
        return Ok(result);
    }
    
    [HttpGet("last/{teamId:guid}/{count:int}")]
    public async Task<IActionResult> GetLast(Guid teamId, int count)
    {
        var result = await _reviewService.GetLast(teamId, count);
        return Ok(result);
    }
}