using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Commands.End;

public sealed record EndCommand(MessageContext MessageContext)
    : IEndDialogCommand;