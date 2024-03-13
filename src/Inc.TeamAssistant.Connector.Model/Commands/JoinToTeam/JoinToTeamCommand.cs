using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;

public sealed record JoinToTeamCommand(MessageContext MessageContext, Guid TeamId)
    : IEndDialogCommand;