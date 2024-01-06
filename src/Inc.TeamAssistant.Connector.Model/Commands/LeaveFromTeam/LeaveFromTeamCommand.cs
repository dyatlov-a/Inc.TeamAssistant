using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;

public sealed record LeaveFromTeamCommand(MessageContext MessageContext, Guid TeamId)
    : IRequest<CommandResult>;