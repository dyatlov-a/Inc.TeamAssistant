using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.Application.Contracts;

public interface IRandomCoffeeRepository
{
    Task<RandomCoffeeEntry?> Find(Guid id, CancellationToken token);
    
    Task<RandomCoffeeEntry?> Find(string pollId, CancellationToken token);
    Task<RandomCoffeeEntry?> Find(long chatId, CancellationToken token);
    
    Task Upsert(RandomCoffeeEntry randomCoffeeEntry, CancellationToken token);
}