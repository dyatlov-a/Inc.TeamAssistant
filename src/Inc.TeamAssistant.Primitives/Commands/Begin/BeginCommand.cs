using MediatR;

namespace Inc.TeamAssistant.Primitives.Commands.Begin;

public sealed record BeginCommand(
    MessageContext MessageContext,
    BotCommandStage NextStage,
    string Command,
    NotificationMessage Notification)
    : IRequest<CommandResult>;