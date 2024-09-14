using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory;

internal sealed class SetEstimateForStoryCommandHandler : IRequestHandler<SetEstimateForStoryCommand, CommandResult>
{
	private readonly IStoryRepository _storyRepository;
    private readonly SummaryByStoryBuilder _summaryBuilder;
    private readonly IMessagesSender _messagesSender;

	public SetEstimateForStoryCommandHandler(
		IStoryRepository storyRepository,
		SummaryByStoryBuilder summaryBuilder,
		IMessagesSender messagesSender)
	{
		_storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
		_summaryBuilder = summaryBuilder ?? throw new ArgumentNullException(nameof(summaryBuilder));
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
	}

    public async Task<CommandResult> Handle(SetEstimateForStoryCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var story = await _storyRepository.Find(command.StoryId, token);
        if (story is null)
	        throw new TeamAssistantUserException(Messages.Appraiser_StoryNotFound, command.StoryId);

        var alreadyEstimate = story.Estimate(command.MessageContext.Person.Id, command.Value);

        await _storyRepository.Upsert(story, token);
        
        switch (alreadyEstimate)
        {
	        case null:
		        throw new TeamAssistantUserException(Messages.Appraiser_MissingTaskForEvaluate);
	        case true:
		        return CommandResult.Empty;
	        default:
		        await _messagesSender.StoryChanged(story.TeamId);
		        return CommandResult.Build(await _summaryBuilder.Build(SummaryByStoryConverter.ConvertTo(story)));
        }
    }
}