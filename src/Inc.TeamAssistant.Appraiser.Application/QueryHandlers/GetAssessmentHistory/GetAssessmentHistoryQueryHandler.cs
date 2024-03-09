using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetAssessmentHistory;

internal sealed class GetAssessmentHistoryQueryHandler
    : IRequestHandler<GetAssessmentHistoryQuery, GetAssessmentHistoryResult>
{
    private readonly IStoryRepository _storyRepository;

    public GetAssessmentHistoryQueryHandler(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
    }

    public async Task<GetAssessmentHistoryResult> Handle(GetAssessmentHistoryQuery query, CancellationToken token)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var history = await _storyRepository.GetAssessmentHistory(query.TeamId, query.Depth, token);

        return new GetAssessmentHistoryResult(history);
    }
}