using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

public sealed record GetHistoryByTeamQuery(Guid TeamId, DateOnly From)
    : IRequest<GetHistoryByTeamResult>;