namespace Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

public sealed record RandomCoffeeHistoryDto(
    DateOnly Created,
    IReadOnlyCollection<PairDto> Pairs,
    string? ExcludedPerson);