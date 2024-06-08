using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

namespace Inc.TeamAssistant.Connector.Application.Contracts;

public interface ITeamReader
{
    Task<IReadOnlyCollection<TeammateDto>> GetTeammates(Guid teamId, CancellationToken token);
}