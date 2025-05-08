using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Model.Queries.Common;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetAvailableTeams;

public sealed record GetAvailableTeamsResult(IReadOnlyCollection<TenantTeamDto> Teams)
    : IWithEmpty<GetAvailableTeamsResult>
{
    public static GetAvailableTeamsResult Empty { get; } = new([]);
}