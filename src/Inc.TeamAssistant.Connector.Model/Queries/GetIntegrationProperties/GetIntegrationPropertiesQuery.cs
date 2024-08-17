using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;

public sealed record GetIntegrationPropertiesQuery(Guid TeamId)
    : IRequest<GetIntegrationPropertiesResult>;