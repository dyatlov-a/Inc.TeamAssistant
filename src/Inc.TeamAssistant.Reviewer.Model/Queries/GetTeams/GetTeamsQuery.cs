using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetTeams;

public sealed record GetTeamsQuery(long PersonId, long ChatId) : IRequest<GetTeamsResult>;