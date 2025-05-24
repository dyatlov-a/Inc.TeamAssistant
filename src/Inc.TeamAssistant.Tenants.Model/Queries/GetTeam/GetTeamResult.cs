using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Model.Queries.Common;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;

public sealed record GetTeamResult(TenantTeamDto Team)
    : IWithEmpty<GetTeamResult>
{
    public static GetTeamResult Empty { get; } = new(new TenantTeamDto(Guid.Empty, string.Empty));
}