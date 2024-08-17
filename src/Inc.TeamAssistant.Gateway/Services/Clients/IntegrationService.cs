using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class IntegrationService : IIntegrationService
{
    private readonly IMediator _mediator;

    public IntegrationService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<ServiceResult<GetIntegrationPropertiesResult>> GetTeamProperties(
        Guid teamId,
        CancellationToken token)
    {
        try
        {
            var result = await _mediator.Send(new GetIntegrationPropertiesQuery(teamId), token);

            return ServiceResult.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetIntegrationPropertiesResult>(ex.Message);
        }
    }

    public async Task SetTeamProperties(SetIntegrationPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, token);
    }
}