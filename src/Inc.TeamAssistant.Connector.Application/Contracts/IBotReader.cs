using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IBotReader
{
    Task<IReadOnlyCollection<Guid>> GetBotIds(CancellationToken token);

    Task<IReadOnlyCollection<Bot>> GetBotsByUser(long userId, CancellationToken token);
    
    Task<Bot?> Find(Guid id, DateTimeOffset now, CancellationToken token);
    
    Task<string> GetToken(Guid botId, CancellationToken token);
}