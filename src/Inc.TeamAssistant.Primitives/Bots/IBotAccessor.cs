namespace Inc.TeamAssistant.Primitives.Bots;

public interface IBotAccessor
{
    Task<string> GetUserName(Guid botId, CancellationToken token = default);
    
    Task<string> GetToken(Guid botId, CancellationToken token = default);
}