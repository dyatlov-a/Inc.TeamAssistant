using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStories;

internal sealed class GetStoriesQueryHandler : IRequestHandler<GetStoriesQuery, GetStoriesResult>
{
    private readonly IStoryRepository _storyRepository;

    public GetStoriesQueryHandler(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
    }

    public async Task<GetStoriesResult> Handle(GetStoriesQuery query, CancellationToken token)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var stories = await _storyRepository.GetStories(query.TeamId, query.AssessmentDate, token);

        return new GetStoriesResult(stories);
    }
}