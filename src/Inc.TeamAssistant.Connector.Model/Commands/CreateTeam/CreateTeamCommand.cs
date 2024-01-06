using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;

public sealed record CreateTeamCommand(
    MessageContext MessageContext,
    string BotName,
    string Name)
    : IRequest<CommandResult>;