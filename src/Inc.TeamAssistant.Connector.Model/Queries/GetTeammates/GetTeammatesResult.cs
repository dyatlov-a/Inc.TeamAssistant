using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

public sealed record GetTeammatesResult(
    bool HasManagerAccess,
    IReadOnlyCollection<TeammateDto> Teammates)
    : IWithEmpty<GetTeammatesResult>
{
    public static GetTeammatesResult Empty { get; } = new(false, Array.Empty<TeammateDto>());
}