namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

public sealed record GetHistoryByTeamResult(
    IReadOnlyCollection<HistoryByTeamItemDto> Review,
    IReadOnlyCollection<HistoryByTeamItemDto> Requests);