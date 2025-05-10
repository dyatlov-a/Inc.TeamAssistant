using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.RemoveTeam;

public sealed record RemoveTeamCommand(Guid TeamId)
    : IRequest;