using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStories;

internal sealed class GetStoriesQueryHandler : IRequestHandler<GetStoriesQuery, GetStoriesResult>
{
    private readonly IStoryReader _reader;

    public GetStoriesQueryHandler(IStoryReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetStoriesResult> Handle(GetStoriesQuery query, CancellationToken token)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var stories = await _reader.GetStories(query.TeamId, query.AssessmentDate, token);

        return new GetStoriesResult(stories);
    }
}