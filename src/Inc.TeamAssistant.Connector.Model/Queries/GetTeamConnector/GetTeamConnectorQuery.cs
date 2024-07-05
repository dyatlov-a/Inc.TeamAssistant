using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;

public sealed record GetTeamConnectorQuery(Guid TeamId)
    : IRequest<GetTeamConnectorResult>;