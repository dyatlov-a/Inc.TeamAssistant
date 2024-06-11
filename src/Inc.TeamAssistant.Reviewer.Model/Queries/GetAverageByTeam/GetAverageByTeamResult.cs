namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;

public sealed record GetAverageByTeamResult(IReadOnlyCollection<ReviewAverageStatsDto> Items);