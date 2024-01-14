using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.SetEstimateForStory;

internal sealed class SetEstimateForStoryCommandHandler : IRequestHandler<SetEstimateForStoryCommand, CommandResult>
{
	private readonly IStoryRepository _storyRepository;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessagesSender _messagesSender;

	public SetEstimateForStoryCommandHandler(
		IStoryRepository storyRepository,
		SummaryByStoryBuilder summaryByStoryBuilder,
		IMessagesSender messagesSender)
	{
		_storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
		_summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
	}

    public async Task<CommandResult> Handle(SetEstimateForStoryCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var story = await _storyRepository.Find(command.StoryId, token);
        if (story is null)
	        throw new ApplicationException($"Story {command.StoryId} was not found.");

        story.Estimate(command.MessageContext.PersonId, command.Value.ToAssessmentValue());

        await _storyRepository.Upsert(story, token);
        
        await _messagesSender.StoryChanged(story.TeamId);

        return CommandResult.Build(await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(story)));
    }
}