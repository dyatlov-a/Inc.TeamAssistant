using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.Help;

public sealed record HelpCommand(MessageContext MessageContext) : IRequest<CommandResult>;