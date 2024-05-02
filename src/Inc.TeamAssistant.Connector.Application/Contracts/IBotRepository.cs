using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IBotRepository
{
    Task<IReadOnlyCollection<Guid>> GetBotIds(CancellationToken token);
    
    Task<Bot?> Find(Guid id, CancellationToken token);
}