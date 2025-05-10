using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;

public sealed record CreateTeamCommand(string Name)
    : IRequest<CreateTeamResult>;