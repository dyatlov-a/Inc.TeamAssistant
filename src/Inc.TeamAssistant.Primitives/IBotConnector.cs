namespace Inc.TeamAssistant.Primitives;

public interface IBotConnector
{
    Task<string> CheckAccess(string botToken, CancellationToken token);
}