using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetActiveStory;

internal sealed class GetActiveStoryQueryHandler : IRequestHandler<GetActiveStoryQuery, GetActiveStoryResult>
{
	private readonly IStoryReader _reader;
	private readonly ITeamAccessor _teamAccessor;
	private readonly IQuickResponseCodeGenerator _codeGenerator;
	private readonly ILinkBuilder _linkBuilder;
	private readonly IBotAccessor _botAccessor;

	public GetActiveStoryQueryHandler(
		IStoryReader reader,
		ITeamAccessor teamAccessor,
		IQuickResponseCodeGenerator codeGenerator,
		ILinkBuilder linkBuilder,
		IBotAccessor botAccessor)
	{
		_reader = reader ?? throw new ArgumentNullException(nameof(reader));
		_teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
		_codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
		_linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
		_botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
	}

	public async Task<GetActiveStoryResult> Handle(GetActiveStoryQuery query, CancellationToken token)
	{
		ArgumentNullException.ThrowIfNull(query);

		var teamContext = await _teamAccessor.GetTeamContext(query.TeamId, token);
		var botContext = await _botAccessor.GetBotContext(teamContext.BotId, token);
		var link = _linkBuilder.BuildLinkForConnect(botContext.UserName, query.TeamId);
		var codeForConnect = _codeGenerator.Generate(link);
		
		var story = await _reader.FindLast(query.TeamId, token);
		var details = story is not null
			? StoryConverter.Convert(story)
			: null;

		return new(teamContext.TeamName, codeForConnect, details);
	}
}