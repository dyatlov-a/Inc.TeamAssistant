using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;

internal sealed record BeginCommand(
    MessageContext MessageContext,
    CommandStage NextStage,
    CurrentTeamContext TeamContext,
    string Command,
    NotificationMessage Notification)
    : IRequest<CommandResult>;