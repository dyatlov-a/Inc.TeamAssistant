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
    Point? Location,
    long? TargetPersonId)
{
    public bool Shared => ChatId != PersonId;
    public string DisplayUsername => string.IsNullOrWhiteSpace(Username) ? FirstName : Username;

    public static MessageContext CreateIdle(Guid botId, long chatId)
    {
        return new MessageContext(
            MessageId: 0,
            BotId: botId,
            BotName: string.Empty,
            Teams: Array.Empty<TeamContext>(),
            Text: string.Empty,
            ChatId: chatId,
            PersonId: 0,
            FirstName: string.Empty,
            Username: null,
            LanguageId: new LanguageId("en"),
            Location: null,
            TargetPersonId: null);
    }
        
    public TeamContext? FindTeam(Guid teamId) => Teams.SingleOrDefault(t => t.Id == teamId);

    public Guid TryParseId(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));

        var parameters = Text.Replace(command, string.Empty, StringComparison.InvariantCultureIgnoreCase);
        return Guid.TryParse(parameters, out var value) ? value : Guid.Empty;
    }
}