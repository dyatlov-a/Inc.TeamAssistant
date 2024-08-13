using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;

public sealed record JoinToTeamCommand(MessageContext MessageContext, Guid TeamId)
    : IDialogCommand;