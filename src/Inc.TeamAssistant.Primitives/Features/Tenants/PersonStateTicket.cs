namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public sealed record PersonStateTicket(Person Person, bool Finished, bool HandRaised);