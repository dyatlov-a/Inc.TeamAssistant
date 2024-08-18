using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;

public sealed record SetIntegrationPropertiesCommand(
    Guid TeamId,
    string AccessToken,
    string ProjectKey,
    long ScrumMasterId)
    : IRequest;