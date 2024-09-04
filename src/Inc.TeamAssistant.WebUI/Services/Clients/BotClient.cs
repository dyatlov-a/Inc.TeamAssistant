using System.Net.Http.Json;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Connector.Model.Queries.GetBots;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
using Inc.TeamAssistant.Constructor.Model.Queries.GetProperties;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class BotClient : IBotService
{
    private readonly HttpClient _client;

    public BotClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<GetBotUserNameResult> Check(GetBotUserNameQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var response = await _client.PostAsJsonAsync("bots/check", query, token);

        response.EnsureSuccessStatusCode();
            
        var result = await response.Content.ReadFromJsonAsync<GetBotUserNameResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetBotResult> GetBotById(Guid botId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetBotResult>($"bots/{botId}", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetBotsResult> GetByUser(long userId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetBotsResult>($"bots/{userId}/user", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetTeammatesResult> GetTeammates(Guid teamId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetTeammatesResult>($"bots/{teamId}/teammates", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetTeamConnectorResult> GetConnector(
        Guid teamId,
        string foreground,
        string background,
        CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);
        
        var result = await _client.GetFromJsonAsync<GetTeamConnectorResult>(
            $"bots/{teamId}/connector?foreground={foreground}&background={background}",
            token);
        
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task RemoveTeammate(RemoveTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _client.PutAsJsonAsync("bots/teammate", command, token);
    }

    public async Task<GetFeaturesResult> GetFeatures(CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetFeaturesResult>("bots/features", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetPropertiesResult> GetProperties(CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<GetPropertiesResult>("bots/properties", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task<GetBotDetailsResult> GetDetails(GetBotDetailsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var response = await _client.PostAsJsonAsync("bots/details", query, token);

        response.EnsureSuccessStatusCode();
            
        var result = await response.Content.ReadFromJsonAsync<GetBotDetailsResult>(token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task Create(CreateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _client.PostAsJsonAsync("bots", command, token);
    }

    public async Task Update(UpdateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _client.PutAsJsonAsync("bots", command, token);
    }

    public async Task Remove(Guid botId, CancellationToken token)
    {
        await _client.DeleteAsync($"bots/{botId}", token);
    }

    public async Task SetDetails(SetBotDetailsCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _client.PutAsJsonAsync("bots/set-details", command, token);
    }
}