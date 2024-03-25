using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;

public sealed record ChangeTeamPropertyCommand(MessageContext MessageContext, Guid TeamId, string Name, string Value)
    : IEndDialogCommand;