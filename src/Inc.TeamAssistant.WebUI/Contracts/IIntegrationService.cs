using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Connector.Model.Commands.DisableIntegration;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IIntegrationService
{
    Task<ServiceResult<GetIntegrationPropertiesResult>> GetTeamProperties(
        Guid teamId,
        CancellationToken token = default);
    
    Task SetTeamProperties(
        SetIntegrationPropertiesCommand command,
        CancellationToken token = default);

    Task DisableIntegration(DisableIntegrationCommand command, CancellationToken token = default);
}