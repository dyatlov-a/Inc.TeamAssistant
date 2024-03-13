using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;

public sealed record MarkMessageForDeleteCommand(MessageContext MainContext, int MessageId)
    : IContinuationCommand;