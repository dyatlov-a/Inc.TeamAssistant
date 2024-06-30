using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;

namespace Inc.TeamAssistant.RandomCoffee.Application.Contracts;

public interface IRandomCoffeeReader
{
    Task<IReadOnlyCollection<RandomCoffeeEntry>> GetByDate(DateOnly date, CancellationToken token);
    
    Task<IReadOnlyCollection<ChatDto>> GetChats(Guid botId, CancellationToken token);
    
    Task<IReadOnlyCollection<RandomCoffeeHistory>> GetHistory(
        Guid botId,
        long chatId,
        int depth,
        CancellationToken token);
}