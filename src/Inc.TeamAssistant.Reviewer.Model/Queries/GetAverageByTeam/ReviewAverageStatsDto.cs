namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;

public sealed record ReviewAverageStatsDto(
    DateOnly Created,
    TimeSpan FirstTouch,
    TimeSpan Review,
    TimeSpan Correction);