using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

public sealed record GetHistoryByTeamQuery(Guid TeamId, int Depth)
    : IRequest<GetHistoryByTeamResult>;