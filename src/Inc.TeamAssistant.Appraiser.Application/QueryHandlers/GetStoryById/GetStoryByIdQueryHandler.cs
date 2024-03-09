using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryById;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStoryById;

internal sealed class GetStoryByIdQueryHandler : IRequestHandler<GetStoryByIdQuery, GetStoryByIdResult?>
{
    private readonly IStoryRepository _storyRepository;

    public GetStoryByIdQueryHandler(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
    }

    public async Task<GetStoryByIdResult?> Handle(GetStoryByIdQuery query, CancellationToken token)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var story = await _storyRepository.Find(query.Id, token);
        
        return story is not null ? new(StoryConverter.Convert(story)) : null;
    }
}