namespace Inc.TeamAssistant.Connector.Application.Parsers;

public interface IInputMessage
{
    string BotName { get; }
    string Text { get; }
    IReadOnlyCollection<long> PersonIds { get; }
}