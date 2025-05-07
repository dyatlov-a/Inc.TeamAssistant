using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;

public sealed record GetTeamQuery(Guid Id)
    : IRequest<GetTeamResult>;