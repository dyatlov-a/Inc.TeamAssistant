using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IBotReader
{
    Task<IReadOnlyCollection<Guid>> GetBotIds(CancellationToken token);

    Task<IReadOnlyCollection<BotDto>> GetBotsByUser(long userId, CancellationToken token);
    
    Task<Guid?> FindBotId(long personId, CancellationToken token);
    
    Task<Bot?> Find(Guid id, DateTimeOffset now, CancellationToken token);
    
    Task<string> GetToken(Guid botId, CancellationToken token);
    
    Task<IReadOnlyCollection<string>> GetFeatures(Guid botId, CancellationToken token);
}