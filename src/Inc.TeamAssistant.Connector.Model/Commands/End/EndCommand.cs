using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.End;

public sealed record EndCommand(MessageContext MessageContext)
    : IEndDialogCommand;