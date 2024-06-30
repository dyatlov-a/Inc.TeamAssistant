using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRandomCoffeeService
{
    Task<ServiceResult<GetChatsResult>> GetChatsByBot(Guid botId, CancellationToken token = default);
    
    Task<ServiceResult<GetHistoryResult>> GetHistory(
        Guid botId,
        long chatId,
        int depth,
        CancellationToken token = default);
}