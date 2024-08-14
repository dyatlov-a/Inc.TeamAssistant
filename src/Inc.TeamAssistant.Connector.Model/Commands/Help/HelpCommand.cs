using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.Help;

public sealed record HelpCommand(MessageContext MessageContext)
    : IDialogCommand;