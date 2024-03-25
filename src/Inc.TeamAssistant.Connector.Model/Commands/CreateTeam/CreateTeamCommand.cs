using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;

public sealed record CreateTeamCommand(
    MessageContext MessageContext,
    string BotName,
    string Name,
    IReadOnlyDictionary<string, string> Properties)
    : IEndDialogCommand;