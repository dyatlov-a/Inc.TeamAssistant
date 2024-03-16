using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.Application.Contracts;

public interface IRandomCoffeeReader
{
    Task<IReadOnlyCollection<RandomCoffeeEntry>> GetByDate(DateOnly date, CancellationToken token);
}