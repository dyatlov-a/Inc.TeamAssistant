using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface ITeamRepository
{
    Task<Team?> Find(Guid teamId, CancellationToken token);
    
    Task Upsert(Team team, CancellationToken token);
    
    Task Remove(Guid teamId, CancellationToken token);
}