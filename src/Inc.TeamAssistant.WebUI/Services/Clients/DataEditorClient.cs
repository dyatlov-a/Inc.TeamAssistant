using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Clients;

internal sealed class DataEditorClient : IDataEditor
{
    private readonly HttpClient _client;

    public DataEditorClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<ServiceResult<string?>> Get(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        var result = await _client.GetFromJsonAsync<ServiceResult<string?>>($"data/{key}", token);
        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task Attach(string key, string data, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        
        try
        {
            await _client.PostAsJsonAsync($"data/{key}", data, token);
        }
        catch
        {
            // ignored
        }
    }

    public async Task Detach(string key, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        try
        {
            await _client.DeleteAsync($"data/{key}", token);
        }
        catch
        {
            // ignored
        }
    }
}