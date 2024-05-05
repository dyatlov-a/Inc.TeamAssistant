using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IBotService
{
    Task<ServiceResult<GetBotsByOwnerResult>> GetBotsByOwner(long ownerId, CancellationToken token = default);
    
    Task<ServiceResult<GetBotUserNameResult>> Check(GetBotUserNameQuery query, CancellationToken token = default);
}