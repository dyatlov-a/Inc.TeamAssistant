using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;

public sealed record BeginCommand(
    MessageContext MessageContext,
    CommandStage NextStage,
    Guid? SelectedTeamId,
    string Command,
    NotificationMessage Notification)
    : IRequest<CommandResult>;