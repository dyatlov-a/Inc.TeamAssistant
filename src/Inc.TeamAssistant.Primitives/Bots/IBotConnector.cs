namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotConnector
{
    Task<string?> GetUsername(string botToken, CancellationToken token);
    
    Task<IReadOnlyCollection<BotDetails>> GetDetails(string botToken, CancellationToken token);

    Task SetCommands(Guid botId, IReadOnlyCollection<string> supportedLanguages, CancellationToken token);
    
    Task SetDetails(string botToken, IReadOnlyCollection<BotDetails> botDetails, CancellationToken token);
}