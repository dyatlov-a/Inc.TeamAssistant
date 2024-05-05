using Inc.TeamAssistant.Constructor.Domain;

namespace Inc.TeamAssistant.Constructor.Application.Contracts;

public interface IBotRepository
{
    Task<IReadOnlyCollection<Bot>> GetBotsByOwner(long ownerId, CancellationToken token);
    
    Task<Bot?> FindById(Guid id, CancellationToken token);
}