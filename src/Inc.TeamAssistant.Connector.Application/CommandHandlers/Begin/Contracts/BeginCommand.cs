using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;

internal sealed record BeginCommand(
    MessageContext MessageContext,
    CommandStage NextStage,
    CurrentTeamContext TeamContext,
    string Command,
    NotificationMessage Notification)
    : IDialogCommand;