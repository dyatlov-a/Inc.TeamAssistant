using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetTaskForReviews;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetTaskForReviews;

internal sealed class GetTaskForReviewsQueryHandler : IRequestHandler<GetTaskForReviewsQuery, GetTaskForReviewsResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;

    public GetTaskForReviewsQueryHandler(ITaskForReviewRepository taskForReviewRepository)
    {
        _taskForReviewRepository = taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
    }

    public async Task<GetTaskForReviewsResult> Handle(GetTaskForReviewsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        
        var taskIds = await _taskForReviewRepository.GetTaskIds(
            TaskForReviewStateRules.ActiveStates,
            cancellationToken);

        return new GetTaskForReviewsResult(taskIds);
    }
}