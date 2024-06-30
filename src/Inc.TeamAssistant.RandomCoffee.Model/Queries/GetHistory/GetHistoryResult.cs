namespace Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

public sealed record GetHistoryResult(IReadOnlyCollection<RandomCoffeeHistoryDto> Items);