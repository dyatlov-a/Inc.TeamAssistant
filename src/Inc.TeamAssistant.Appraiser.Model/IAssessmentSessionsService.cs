using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model;

public interface IAssessmentSessionsService
{
	Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        AssessmentSessionId assessmentSessionId,
        int width,
        int height,
        bool drawQuietZones,
		CancellationToken cancellationToken = default);

    Task<ServiceResult<GetLinkForConnectResult>> GetLinkForConnect(
        int width,
        int height,
        bool drawQuietZones,
        CancellationToken cancellationToken = default);
}