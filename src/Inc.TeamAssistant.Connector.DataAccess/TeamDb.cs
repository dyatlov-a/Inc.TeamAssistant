namespace Inc.TeamAssistant.Connector.DataAccess;

public sealed record TeamDb(
    Guid Id,
    Guid BotId,
    long ChatId,
    string Name,
    IReadOnlyDictionary<string, string> Properties,
    long OwnerId,
    string OwnerName,
    string? OwnerUsername);