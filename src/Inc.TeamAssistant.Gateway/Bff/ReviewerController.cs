using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

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
    
    [HttpGet("history/{teamId:guid}/{from}")]
    public async Task<IActionResult> GetHistory(Guid teamId, DateOnly from)
    {
        var result = await _reviewService.GetHistory(teamId, from);
        return Ok(result);
    }

    [HttpGet("average/{teamId:guid}/{from}")]
    public async Task<IActionResult> GetAverage(Guid teamId, DateOnly from)
    {
        var result = await _reviewService.GetAverage(teamId, from);
        return Ok(result);
    }
    
    [HttpGet("last/{teamId:guid}/{from}")]
    public async Task<IActionResult> GetLast(Guid teamId, DateOnly from)
    {
        var result = await _reviewService.GetLast(teamId, from);
        return Ok(result);
    }
}