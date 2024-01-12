using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.Begin;

public sealed record BeginCommand(
    MessageContext MessageContext,
    object NextStage,
    Guid? SelectedTeamId,
    string Command,
    NotificationMessage Notification)
    : IRequest<CommandResult>;