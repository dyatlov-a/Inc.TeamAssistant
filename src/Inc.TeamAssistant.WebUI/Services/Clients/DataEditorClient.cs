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

    public async Task<ServiceResult<string?>> Get(Guid dataId, CancellationToken token)
    {
        var result = await _client.GetFromJsonAsync<ServiceResult<string?>>($"user-data/{dataId:N}", token);

        if (result is null)
            throw new TeamAssistantException("Parse response with error.");

        return result;
    }

    public async Task Attach(Guid dataId, string data, CancellationToken token)
    {
        try
        {
            await _client.PostAsJsonAsync($"user-data/{dataId:N}", data, token);
        }
        catch
        {
            // ignored
        }
    }
}