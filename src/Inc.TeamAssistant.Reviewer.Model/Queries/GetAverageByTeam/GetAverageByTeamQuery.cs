using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;

public sealed record GetAverageByTeamQuery(Guid TeamId, DateOnly From)
    : IRequest<GetAverageByTeamResult>;