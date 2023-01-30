using System.Net.Http.Json;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Common.Messages.Client;

public sealed class MessageServiceClient : IMessageService
{
    private readonly HttpClient _client;

    public MessageServiceClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<ServiceResult<Dictionary<string, Dictionary<string, string>>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<Dictionary<string, Dictionary<string, string>>>>(
                "resources",
                cancellationToken: cancellationToken);

            if (result is null)
                throw new ApplicationException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<Dictionary<string, Dictionary<string, string>>>(ex.Message);
        }
    }
}