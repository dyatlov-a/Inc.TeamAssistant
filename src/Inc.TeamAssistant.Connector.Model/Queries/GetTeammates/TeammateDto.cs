namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

public sealed record TeammateDto(Guid TeamId, long PersonId, string Name);