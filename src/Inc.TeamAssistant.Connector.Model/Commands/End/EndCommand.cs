using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.End;

public sealed record EndCommand(MessageContext MessageContext)
    : IRequest<CommandResult>;