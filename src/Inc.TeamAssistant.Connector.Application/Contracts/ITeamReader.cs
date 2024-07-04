using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface ITeamReader
{
    Task<bool> HasManagerAccess(Guid teamId, long personId, CancellationToken token);
    
    Task<IReadOnlyCollection<TeammateDto>> GetTeammates(Guid teamId, CancellationToken token);
}