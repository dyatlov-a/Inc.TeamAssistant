namespace Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

public sealed record PairDto(
    string FirstName,
    string? FirstUserName,
    string SecondName,
    string? SecondUserName);