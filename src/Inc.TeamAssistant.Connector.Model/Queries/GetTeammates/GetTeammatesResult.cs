namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

public sealed record GetTeammatesResult(IReadOnlyCollection<TeammateDto> Teammates);