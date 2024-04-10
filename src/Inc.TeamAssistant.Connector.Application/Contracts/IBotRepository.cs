using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface IBotRepository
{
    Task<IReadOnlyCollection<Bot>> GetAll(CancellationToken token);

    Task<string> GetBotName(Guid id, CancellationToken token);
    
    Task<Bot?> Find(Guid id, CancellationToken token);
}