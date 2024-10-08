using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;

public sealed record LeaveFromTeamCommand(MessageContext MessageContext, Guid TeamId)
    : IEndDialogCommand;