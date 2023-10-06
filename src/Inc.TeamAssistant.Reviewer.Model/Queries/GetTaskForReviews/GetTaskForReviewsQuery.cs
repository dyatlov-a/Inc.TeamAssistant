using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetTaskForReviews;

public sealed record GetTaskForReviewsQuery : IRequest<GetTaskForReviewsResult>;