namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;

public sealed record GetTeamConnectorResult(
    string TeamName,
    string LinkForConnect,
    string Code);