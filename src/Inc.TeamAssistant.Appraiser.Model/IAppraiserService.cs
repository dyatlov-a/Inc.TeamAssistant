using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

namespace Inc.TeamAssistant.Appraiser.Model;

public interface IAppraiserService
{
	Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(
        Guid teamId,
		CancellationToken cancellationToken = default);

    Task<ServiceResult<GetLinkForConnectResult>> GetLinkForConnect(CancellationToken cancellationToken = default);
}