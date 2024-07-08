namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotConnector
{
    Task<string?> GetUsername(string botToken, CancellationToken token);

    Task Setup(Guid botId, CancellationToken token);
}