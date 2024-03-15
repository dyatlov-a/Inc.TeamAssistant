using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryById;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStoryById;

internal sealed class GetStoryByIdQueryHandler : IRequestHandler<GetStoryByIdQuery, GetStoryByIdResult?>
{
    private readonly IStoryReader _storyReader;

    public GetStoryByIdQueryHandler(IStoryReader storyReader)
    {
        _storyReader = storyReader ?? throw new ArgumentNullException(nameof(storyReader));
    }

    public async Task<GetStoryByIdResult?> Handle(GetStoryByIdQuery query, CancellationToken token)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var story = await _storyReader.Find(query.Id, token);
        
        return story is not null ? new(StoryConverter.Convert(story)) : null;
    }
}