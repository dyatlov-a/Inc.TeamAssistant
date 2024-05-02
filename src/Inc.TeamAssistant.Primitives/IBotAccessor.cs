namespace Inc.TeamAssistant.Primitives;

public interface IBotAccessor
{
    Task<string> GetUserName(Guid botId, CancellationToken token);
}