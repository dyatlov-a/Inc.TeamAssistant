namespace Inc.TeamAssistant.Reviewer.Model.Queries.GetTaskForReviews;

public sealed record GetTaskForReviewsResult(IReadOnlyCollection<Guid> TaskIds);