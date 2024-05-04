using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IBotService
{
    Task<ServiceResult<GetBotsByOwnerResult>> GetBotsByOwner(long ownerId, CancellationToken token = default);
}