using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;

public sealed record MarkMessageForDeleteCommand(MessageContext MainContext, int MessageId)
    : IContinuationCommand;