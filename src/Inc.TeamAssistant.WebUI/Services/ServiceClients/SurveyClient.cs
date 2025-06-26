using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class SurveyClient : ISurveyService
{
    private readonly HttpClient _client;

    public SurveyClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    public async Task<GetSurveyTemplatesResult> GetSurveyTemplates(CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetSurveyTemplatesResult>("survey/templates", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }
}