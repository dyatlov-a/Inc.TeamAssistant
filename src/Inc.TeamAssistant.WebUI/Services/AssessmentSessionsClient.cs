using System.Net.Http.Json;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.WebUI.Services;

internal sealed class AssessmentSessionsClient : IAssessmentSessionsService
{
	private readonly HttpClient _client;

    public AssessmentSessionsClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

	public async Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        AssessmentSessionId assessmentSessionId,
        int width,
        int height,
        bool drawQuietZones,
		CancellationToken cancellationToken)
	{
		try
		{
			var result = await _client.GetFromJsonAsync<ServiceResult<GetStoryDetailsResult?>>(
				$"sessions/story/{assessmentSessionId.Value}?width={width}&height={height}&drawQuietZones={drawQuietZones}",
				cancellationToken);

			if (result is null)
				throw new ApplicationException("Parse response with error.");

			return result;
		}
		catch (Exception ex)
		{
			return ServiceResult.Failed<GetStoryDetailsResult?>(ex.Message);
		}
	}

    public async Task<ServiceResult<GetLinkForConnectResult>> GetLinkForConnect(
        int width,
        int height,
        bool drawQuietZones,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<ServiceResult<GetLinkForConnectResult>>(
                $"sessions/link-for-connect?width={width}&height={height}&drawQuietZones={drawQuietZones}",
                cancellationToken);

            if (result is null)
                throw new ApplicationException("Parse response with error.");

            return result;
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLinkForConnectResult>(ex.Message);
        }
    }
}