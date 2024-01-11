namespace Inc.TeamAssistant.Primitives;

public sealed record MessageContext(
    Guid BotId,
    string BotName,
    IReadOnlyList<(Guid Id, long ChatId, string Name)> Teams,
    string Cmd,
    string Text,
    long ChatId,
    long PersonId,
    string FirstName,
    string? Username,
    int MessageId,
    LanguageId LanguageId,
    BotCommandStage? CurrentCommandStage,
    UserIdentity? TargetUser)
{
    public bool Shared => ChatId != PersonId;
    public string DisplayUsername => string.IsNullOrWhiteSpace(Username) ? FirstName : Username;

    public (Guid Id, long ChatId, string Name)? FindTeam(Guid teamId) => Teams.SingleOrDefault(t => t.Id == teamId);
}