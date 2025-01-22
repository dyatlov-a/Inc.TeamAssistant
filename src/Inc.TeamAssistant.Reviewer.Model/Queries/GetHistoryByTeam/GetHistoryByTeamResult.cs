using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

public sealed record GetHistoryByTeamResult(
    IReadOnlyCollection<HistoryByTeamItemDto> Review,
    IReadOnlyCollection<HistoryByTeamItemDto> Requests)
    : IWithEmpty<GetHistoryByTeamResult>
{
    public static GetHistoryByTeamResult Empty { get; } = new(Array.Empty<HistoryByTeamItemDto>(), Array.Empty<HistoryByTeamItemDto>());
}