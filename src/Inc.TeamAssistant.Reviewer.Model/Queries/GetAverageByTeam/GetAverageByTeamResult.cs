using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;

public sealed record GetAverageByTeamResult(IReadOnlyCollection<ReviewAverageStatsDto> Items)
    : IWithEmpty<GetAverageByTeamResult>
{
    public static GetAverageByTeamResult Empty { get; } = new(Array.Empty<ReviewAverageStatsDto>());
}