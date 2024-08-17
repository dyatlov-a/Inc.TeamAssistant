using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class IntegrationClient : IIntegrationService
{
    private readonly HttpClient _client;

    public IntegrationClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<ServiceResult<GetIntegrationPropertiesResult>> GetTeamProperties(
        Guid teamId,
        CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetIntegrationPropertiesResult>>(
                $"integrations/{teamId:N}/team",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetIntegrationPropertiesResult>(ex.Message);
        }
    }

    public async Task SetTeamProperties(SetIntegrationPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _client.PutAsJsonAsync("integrations", command, token);
    }
}