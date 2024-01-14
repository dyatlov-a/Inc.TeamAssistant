namespace Inc.TeamAssistant.Primitives;

public sealed record MessageContext(
    int MessageId,
    Guid BotId,
    string BotName,
    IReadOnlyList<TeamContext> Teams,
    string Text,
    long ChatId,
    long PersonId,
    string FirstName,
    string? Username,
    LanguageId LanguageId,
    (double Longitude, double Latitude)? Location)
{
    public bool Shared => ChatId != PersonId;
    public string DisplayUsername => string.IsNullOrWhiteSpace(Username) ? FirstName : Username;

    public TeamContext? FindTeam(Guid teamId) => Teams.SingleOrDefault(t => t.Id == teamId);
}