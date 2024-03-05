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
    Point? Location)
{
    public bool Shared => ChatId != PersonId;
    public string DisplayUsername => string.IsNullOrWhiteSpace(Username) ? FirstName : Username;

    public TeamContext? FindTeam(Guid teamId) => Teams.SingleOrDefault(t => t.Id == teamId);

    public Guid TryParseId(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));

        var parameters = Text.Replace(command, string.Empty, StringComparison.InvariantCultureIgnoreCase);
        return Guid.TryParse(parameters, out var value) ? value : Guid.Empty;
    }

    public bool IsCancel() => "/cancel".Equals(Text, StringComparison.InvariantCultureIgnoreCase);
}