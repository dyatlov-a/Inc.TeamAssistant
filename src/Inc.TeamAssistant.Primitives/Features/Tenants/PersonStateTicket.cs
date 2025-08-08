namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public sealed record PersonStateTicket(
    Person Person,
    bool IsOnline,
    int TotalVote,
    bool Finished,
    bool HandRaised);