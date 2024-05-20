namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotAccessor
{
    Task<BotContext> GetBotContext(Guid botId, CancellationToken token = default);
    
    Task<string> GetToken(Guid botId, CancellationToken token = default);
}