using Inc.TeamAssistant.Connector.Model.Commands.DisableIntegration;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
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
    
    [HttpPut]
    public async Task<IActionResult> SetTeamProperties(
        SetIntegrationPropertiesCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _integrationService.SetTeamProperties(command, token);

        return Ok();
    }
    
    [HttpPut("disable")]
    public async Task<IActionResult> DisableIntegration(
        DisableIntegrationCommand command,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _integrationService.DisableIntegration(command, token);

        return Ok();
    }
}