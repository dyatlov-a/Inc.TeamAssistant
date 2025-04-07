namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed record ReviewTicket(
    long ReviewerId,
    long OwnerId,
    DateTimeOffset Created);