using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class AppraiserClient : IAppraiserService
{
	private readonly HttpClient _client;

    public AppraiserClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

	public async Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        Guid teamId,
		CancellationToken cancellationToken)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetStoryDetailsResult?>>(
				$"sessions/story/{teamId}",
				cancellationToken);

			if (result is null)
				throw new TeamAssistantException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoryDetailsResult?>(ex.Message);
		}
	}

    public async Task<ServiceResult<GetLinkForConnectResult>> GetLinkForConnect(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetLinkForConnectResult>>(
                "sessions/link-for-connect",
                cancellationToken);

            if (result is null)
                throw new TeamAssistantException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLinkForConnectResult>(ex.Message);
        }
    }
}