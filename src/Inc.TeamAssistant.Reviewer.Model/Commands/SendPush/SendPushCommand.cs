using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.SendPush;

public sealed record SendPushCommand(MessageContext MessageContext, Guid TaskId)
    : IDialogCommand;