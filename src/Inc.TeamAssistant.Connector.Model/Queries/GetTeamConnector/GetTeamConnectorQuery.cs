using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;

public sealed record GetTeamConnectorQuery(
    Guid TeamId,
    string Foreground,
    string Background)
    : IRequest<GetTeamConnectorResult>;