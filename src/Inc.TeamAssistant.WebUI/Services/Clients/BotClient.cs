using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;
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
    
    public async Task<ServiceResult<GetBotsByOwnerResult>> GetBotsByOwner(long ownerId, CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetBotsByOwnerResult>>(
                $"bots/{ownerId}",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotsByOwnerResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetBotUserNameResult>> Check(GetBotUserNameQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        try
        {
            var response = await _client.PostAsJsonAsync("bots/check", query, token);

            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ServiceResult<GetBotUserNameResult>>(token);
            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotUserNameResult>(ex.Message);
        }
    }

    public async Task<ServiceResult<GetBotResult?>> GetBotById(Guid botId, CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetBotResult?>>($"bots/{botId}", token);
            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetBotResult?>(ex.Message);
        } 
    }

    public async Task<ServiceResult<GetFeaturesResult>> GetFeatures(CancellationToken token)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetFeaturesResult>>(
                "bots/features",
                token);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetFeaturesResult>(ex.Message);
        }
    }

    public async Task Create(CreateBotCommand command, CancellationToken token)
    {
        try
        {
            await _client.PostAsJsonAsync("bots", command, token);
        }
        catch
        {
            // ignored
        }
    }

    public async Task Update(UpdateBotCommand command, CancellationToken token)
    {
        try
        {
            await _client.PutAsJsonAsync("bots", command, token);
        }
        catch
        {
            // ignored
        }
    }

    public async Task Remove(Guid botId, CancellationToken token)
    {
        try
        {
            await _client.DeleteAsync($"bots/{botId}", token);
        }
        catch
        {
            // ignored
        }
    }
}