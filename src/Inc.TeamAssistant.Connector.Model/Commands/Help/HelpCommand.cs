using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Commands.Help;

public sealed record HelpCommand(MessageContext MessageContext)
    : IEndDialogCommand;