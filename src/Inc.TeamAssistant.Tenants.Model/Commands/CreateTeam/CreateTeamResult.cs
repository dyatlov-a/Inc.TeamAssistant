using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;

public sealed record CreateTeamResult(Guid TeamId)
    : IWithEmpty<CreateTeamResult>
{
    public static CreateTeamResult Empty { get; } = new(Guid.Empty);
}