using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.MarkMessageForDelete;

public sealed record MarkMessageForDeleteCommand(MessageContext MessageContext, int MessageId)
    : IRequest<CommandResult>;