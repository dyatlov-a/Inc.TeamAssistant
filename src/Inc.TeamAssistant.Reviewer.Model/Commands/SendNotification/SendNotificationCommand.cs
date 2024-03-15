using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.SendNotification;

public sealed record SendNotificationCommand(MessageContext MessageContext, Guid TaskId)
    : IDialogCommand;