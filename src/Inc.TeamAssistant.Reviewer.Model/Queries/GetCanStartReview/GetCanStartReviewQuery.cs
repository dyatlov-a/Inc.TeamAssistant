using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetCanStartReview;

public sealed record GetCanStartReviewQuery(Guid TeamId) : IRequest<GetCanStartReviewResult>;