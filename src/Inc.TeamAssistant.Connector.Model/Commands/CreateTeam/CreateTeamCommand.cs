using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;

public sealed record CreateTeamCommand(MessageContext MessageContext, string BotName, string Name)
    : IEndDialogCommand;