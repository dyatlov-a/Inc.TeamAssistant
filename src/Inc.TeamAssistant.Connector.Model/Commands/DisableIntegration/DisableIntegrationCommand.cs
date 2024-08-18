using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.DisableIntegration;

public sealed record DisableIntegrationCommand(Guid TeamId)
    : IRequest;