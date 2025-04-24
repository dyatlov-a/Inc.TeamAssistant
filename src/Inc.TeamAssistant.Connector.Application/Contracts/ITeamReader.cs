using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface ITeamReader
{
    Task<IReadOnlyCollection<TeammateDto>> GetTeammates(Guid teamId, CancellationToken token);
    
    Task<IReadOnlyCollection<Person>> GetTeammates(
        Guid teamId,
        DateTimeOffset now,
        bool? canFinalize,
        CancellationToken token);
}