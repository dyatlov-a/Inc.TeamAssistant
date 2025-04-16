using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Extensions;

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

        var story = await command.StoryId.Required(_storyRepository.Find, token);
        var alreadyEstimated = story.Estimate(command.MessageContext.Person.Id, command.Value);

        await _storyRepository.Upsert(story, token);
        
        switch (alreadyEstimated)
        {
	        case null:
		        throw new TeamAssistantUserException(Messages.Appraiser_MissingTaskForEvaluate);
	        case true:
		        return CommandResult.Empty;
	        default:
		        var notification = _summaryBuilder.Build(SummaryByStoryConverter.ConvertTo(story));
		        await _messagesSender.StoryChanged(story.TeamId);
		        return CommandResult.Build(notification);
        }
    }
}