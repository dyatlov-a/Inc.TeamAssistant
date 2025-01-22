using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;

public sealed record GetTeamConnectorResult(
    string TeamName,
    string LinkForConnect,
    string Code)
    : IWithEmpty<GetTeamConnectorResult>
{
    public static GetTeamConnectorResult Empty { get; } = new(string.Empty, string.Empty, string.Empty);
}