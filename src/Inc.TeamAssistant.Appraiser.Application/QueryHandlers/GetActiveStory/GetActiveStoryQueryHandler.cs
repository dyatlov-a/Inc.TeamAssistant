using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetActiveStory;

internal sealed class GetActiveStoryQueryHandler : IRequestHandler<GetActiveStoryQuery, GetActiveStoryResult>
{
	private readonly IStoryReader _reader;
	private readonly ITeamLinkBuilder _teamLinkBuilder;

	public GetActiveStoryQueryHandler(IStoryReader reader, ITeamLinkBuilder teamLinkBuilder)
	{
		_reader = reader ?? throw new ArgumentNullException(nameof(reader));
		_teamLinkBuilder = teamLinkBuilder ?? throw new ArgumentNullException(nameof(teamLinkBuilder));
	}

	public async Task<GetActiveStoryResult> Handle(GetActiveStoryQuery query, CancellationToken token)
	{
		ArgumentNullException.ThrowIfNull(query);
		
		var teamConnector = await _teamLinkBuilder.GenerateTeamConnector(query.TeamId, token);
		
		var story = await _reader.FindLast(query.TeamId, token);
		var details = story is not null
			? StoryConverter.Convert(story)
			: null;

		return new(teamConnector.TeamName, teamConnector.Code, details);
	}
}