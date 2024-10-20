using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AcceptEstimate;

internal sealed class AcceptEstimateCommandHandler : IRequestHandler<AcceptEstimateCommand, CommandResult>
{
	private readonly IStoryRepository _storyRepository;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessagesSender _messagesSender;
    private readonly ITeamAccessor _teamAccessor;

	public AcceptEstimateCommandHandler(
		IStoryRepository storyRepository,
		SummaryByStoryBuilder summaryByStoryBuilder,
		IMessagesSender messagesSender,
		ITeamAccessor teamAccessor)
	{
		_storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
		_summaryByStoryBuilder =
			summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
		_teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
	}

	public async Task<CommandResult> Handle(AcceptEstimateCommand command, CancellationToken token)
	{
        ArgumentNullException.ThrowIfNull(command);

        var story = await _storyRepository.Find(command.StoryId, token);
		if (story is null)
			throw new TeamAssistantUserException(Messages.Appraiser_StoryNotFound, command.StoryId);
		
		var hasManagerAccess = await _teamAccessor.HasManagerAccess(story.TeamId, command.MessageContext.Person.Id, token);

		var totalEstimation = story.Accept(command.MessageContext.Person.Id, hasManagerAccess, command.Value);

		await _storyRepository.Upsert(story, token);

		await _messagesSender.StoryAccepted(story.TeamId, totalEstimation.DisplayValue);

		return CommandResult.Build(await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(story)));
    }
}