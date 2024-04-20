using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.RemoveTeam;

public sealed record RemoveTeamCommand(MessageContext MessageContext, Guid TeamId)
    : IEndDialogCommand;