using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

public sealed record GetTeammatesQuery(Guid TeamId) : IRequest<GetTeammatesResult>;