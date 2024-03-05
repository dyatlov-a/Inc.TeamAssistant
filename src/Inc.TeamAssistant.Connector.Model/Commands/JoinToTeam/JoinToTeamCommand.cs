using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;

public sealed record JoinToTeamCommand(MessageContext MessageContext, Guid TeamId)
    : IRequest<CommandResult>;