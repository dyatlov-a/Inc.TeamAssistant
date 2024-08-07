using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

public sealed record AttachMessageCommand(MessageContext MainContext, Guid TaskId, int MessageId, string MessageType)
    : IContinuationCommand;