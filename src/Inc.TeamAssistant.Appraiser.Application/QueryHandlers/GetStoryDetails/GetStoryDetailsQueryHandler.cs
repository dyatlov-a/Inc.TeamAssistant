using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;
using MediatR;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStoryDetails;

internal sealed class GetStoryDetailsQueryHandler : IRequestHandler<GetStoryDetailsQuery, GetStoryDetailsResult?>
{
	private readonly IStoryReader _reader;
    private readonly IQuickResponseCodeGenerator _codeGenerator;
    private readonly ILinkBuilder _linkBuilder;
    private readonly IBotAccessor _botAccessor;

	public GetStoryDetailsQueryHandler(
		IStoryReader reader,
        IQuickResponseCodeGenerator codeGenerator,
        ILinkBuilder linkBuilder,
		IBotAccessor botAccessor)
	{
		_reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
	}

	public async Task<GetStoryDetailsResult?> Handle(GetStoryDetailsQuery query, CancellationToken token)
	{
		if (query is null)
			throw new ArgumentNullException(nameof(query));

		var story = await _reader.FindLast(query.TeamId, token);
		if (story is null)
			throw new TeamAssistantException($"Story for {query.TeamId} was not found.");

		var botName = await _botAccessor.GetUserName(story.BotId, token);
		var link = _linkBuilder.BuildLinkForConnect(botName, story.TeamId);
		var code = _codeGenerator.Generate(link);

		return new(StoryConverter.Convert(story), code);
	}
}