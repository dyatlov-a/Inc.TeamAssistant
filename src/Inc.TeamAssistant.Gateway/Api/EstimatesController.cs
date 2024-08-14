using Inc.TeamAssistant.Gateway.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Api;

[ApiController]
[Route("api/v1/estimates")]
public sealed class EstimatesController : ControllerBase
{
    private readonly ILogger<EstimatesController> _logger;

    public EstimatesController(ILogger<EstimatesController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public IActionResult Create([FromBody]StartEstimateRequest startEstimate)
    {
        ArgumentNullException.ThrowIfNull(startEstimate);

        _logger.LogWarning("Created estimate from jira CreateEstimateRequest: {@StartEstimate}", startEstimate);
        
        return Ok();
    }
}