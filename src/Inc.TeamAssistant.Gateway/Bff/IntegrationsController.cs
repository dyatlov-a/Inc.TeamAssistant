using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("integrations")]
public sealed class IntegrationsController : ControllerBase
{
    private readonly IIntegrationService _integrationService;

    public IntegrationsController(IIntegrationService integrationService)
    {
        _integrationService = integrationService ?? throw new ArgumentNullException(nameof(integrationService));
    }
    
    [HttpGet("{teamId}/team")]
    public async Task<IActionResult> GetTeamProperties(Guid teamId, CancellationToken token)
    {
        var result = await _integrationService.GetTeamProperties(teamId, token);

        return Ok(result);
    }
}