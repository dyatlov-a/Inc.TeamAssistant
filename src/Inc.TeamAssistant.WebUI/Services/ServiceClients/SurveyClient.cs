using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class SurveyClient : ISurveyService
{
    private readonly HttpClient _client;

    public SurveyClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetSurveyStateResult> GetSurveyState(Guid roomId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetSurveyStateResult>($"survey/{roomId:N}/state", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task Start(StartSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("survey", command, token);
        
        await response.HandleValidation(token);
    }

    public async Task Finish(FinishSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("survey", command, token);
        
        await response.HandleValidation(token);
    }
}