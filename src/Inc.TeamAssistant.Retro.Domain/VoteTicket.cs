namespace Inc.TeamAssistant.Retro.Domain;

public sealed record VoteTicket(Guid ItemId, long PersonId, int Vote);