using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IBotService
{
    Task<ServiceResult<GetBotsByOwnerResult>> GetBotsByOwner(long ownerId, CancellationToken token = default);
    
    Task<ServiceResult<GetBotUserNameResult>> Check(GetBotUserNameQuery query, CancellationToken token = default);

    Task<ServiceResult<GetBotResult?>> GetBotById(Guid botId, CancellationToken token = default);
    
    Task<ServiceResult<GetFeaturesResult>> GetFeatures(CancellationToken token = default);

    Task Create(CreateBotCommand command, CancellationToken token = default);

    Task Update(UpdateBotCommand command, CancellationToken token = default);
    
    Task Remove(Guid botId, CancellationToken token = default);
}