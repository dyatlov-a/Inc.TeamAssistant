using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;

public sealed record SetIntegrationPropertiesCommand(
    Guid TeamId,
    string ProjectKey,
    long ScrumMasterId)
    : IRequest;