using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
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
        ArgumentNullException.ThrowIfNull(query);

        var stories = await _reader.GetStories(query.TeamId, query.AssessmentDate, token);
        var results = stories.Select(StoryConverter.Convert).ToArray(); 

        return new GetStoriesResult(results);
    }
}