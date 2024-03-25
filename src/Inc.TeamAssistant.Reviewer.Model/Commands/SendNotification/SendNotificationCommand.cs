using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.SendNotification;

public sealed record SendNotificationCommand(MessageContext MessageContext, Guid TaskId)
    : IDialogCommand;