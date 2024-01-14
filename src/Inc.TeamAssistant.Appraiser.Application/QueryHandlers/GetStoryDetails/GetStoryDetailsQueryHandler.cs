using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStoryDetails;

internal sealed class GetStoryDetailsQueryHandler : IRequestHandler<GetStoryDetailsQuery, GetStoryDetailsResult?>
{
	private readonly IStoryRepository _storyRepository;
    private readonly IQuickResponseCodeGenerator _codeGenerator;
    private readonly ILinkBuilder _linkBuilder;

	public GetStoryDetailsQueryHandler(
		IStoryRepository storyRepository,
        IQuickResponseCodeGenerator codeGenerator,
        ILinkBuilder linkBuilder)
	{
		_storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
	}

	public async Task<GetStoryDetailsResult?> Handle(GetStoryDetailsQuery query, CancellationToken token)
	{
		if (query is null)
			throw new ArgumentNullException(nameof(query));

		var story = await _storyRepository.FindLast(query.TeamId, token);
        return Get(story ?? Story.Empty);
    }

    private GetStoryDetailsResult Get(Story story)
    {
        if (story is null)
            throw new ArgumentNullException(nameof(story));

        var estimateEnded = story.EstimateEnded();
        var link = _linkBuilder.BuildLinkForConnect(story.TeamId);
        var code = _codeGenerator.Generate(link);

        var items = story.StoryForEstimates
            .Select(e => new StoryForEstimateDto(
                e.ParticipantDisplayName,
                estimateEnded ? e.Value.ToDisplayValue(story.StoryType) : e.Value.ToDisplayHasValue()))
            .ToArray();

        return new(
	        story.Title,
            code,
            StorySelected: story != Story.Empty,
            new StoryDetails(story.Title, story.Links.ToArray()),
            items,
            story.GetTotal().ToDisplayValue(estimateEnded, story.StoryType));
    }
}