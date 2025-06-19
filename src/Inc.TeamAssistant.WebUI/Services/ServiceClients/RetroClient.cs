using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Commands.StartRetro;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroTemplates;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;

namespace Inc.TeamAssistant.WebUI.Services.ServiceClients;

internal sealed class RetroClient : IRetroService
{
    private readonly HttpClient _client;

    public RetroClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetRetroStateResult> GetRetroState(Guid teamId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRetroStateResult>($"retro/{teamId:N}/state", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task StartRetro(StartRetroCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PostAsJsonAsync("retro", command, token);
        
        await response.HandleValidation(token);
    }

    public async Task MoveToNextRetroState(MoveToNextRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("retro", command, token);

        await response.HandleValidation(token);
    }

    public async Task<GetActionItemsResult> GetActionItems(Guid teamId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetActionItemsResult>($"retro/{teamId:N}/actions", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task ChangeActionItem(ChangeActionItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("retro/actions", command, token);

        await response.HandleValidation(token);
    }

    public async Task<GetRetroAssessmentResult> GetRetroAssessment(Guid sessionId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRetroAssessmentResult>(
            $"retro/{sessionId:N}/assessments",
            token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task SetRetroAssessment(SetRetroAssessmentCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("retro/assessments", command, token);

        await response.HandleValidation(token);
    }

    public async Task<GetRetroTemplatesResult> GetRetroTemplates(CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetRetroTemplatesResult>(
            $"retro/templates",
            token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task ChangeRetroProperties(ChangeRetroPropertiesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var response = await _client.PutAsJsonAsync("retro/properties", command, token);

        await response.HandleValidation(token);
    }
}