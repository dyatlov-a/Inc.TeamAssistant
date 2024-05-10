namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotConnector
{
    Task<string> CheckAccess(string botToken, CancellationToken token);
}