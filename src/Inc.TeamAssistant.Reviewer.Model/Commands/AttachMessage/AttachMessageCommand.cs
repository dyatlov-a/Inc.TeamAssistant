using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

public sealed record AttachMessageCommand(MessageContext MainContext, Guid TaskId, int MessageId)
    : IContinuationCommand;