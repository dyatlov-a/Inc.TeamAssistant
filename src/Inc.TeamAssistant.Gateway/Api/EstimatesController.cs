using Inc.TeamAssistant.Gateway.Auth;
using Inc.TeamAssistant.Gateway.Services.Integrations;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Api;

[ApiController]
[Route("api/v1/estimates")]
[ApiAuthorize]
public sealed class EstimatesController : ControllerBase
{
    private readonly EstimatesService _estimatesService;

    public EstimatesController(EstimatesService estimatesService)
    {
        _estimatesService = estimatesService ?? throw new ArgumentNullException(nameof(estimatesService));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]StartEstimateRequest startEstimate)
    {
        ArgumentNullException.ThrowIfNull(startEstimate);

        await _estimatesService.StartEstimate(startEstimate, CancellationToken.None);
        
        return Ok();
    }
}