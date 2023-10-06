namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetTeams;

public sealed record GetTeamsResult(IReadOnlyCollection<TeamDto> Teams);