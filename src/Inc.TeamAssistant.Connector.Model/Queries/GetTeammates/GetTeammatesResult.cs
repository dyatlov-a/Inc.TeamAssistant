namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

public sealed record GetTeammatesResult(
    bool HasManagerAccess,
    IReadOnlyCollection<TeammateDto> Teammates);