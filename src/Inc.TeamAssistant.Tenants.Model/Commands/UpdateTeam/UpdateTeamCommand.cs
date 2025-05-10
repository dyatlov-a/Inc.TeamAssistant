using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;

public sealed record UpdateTeamCommand(Guid Id, string Name)
    : IRequest;