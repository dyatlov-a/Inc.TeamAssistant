namespace Inc.TeamAssistant.Primitives.Integrations;

public sealed record IntegrationContext(
    Guid TeamId,
    IReadOnlyDictionary<string, string> TeamProperties,
    Guid BotId,
    long OwnerId,
    long ChatId);