using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ReVoteEstimate;

internal sealed class ReVoteEstimateCommandHandler : IRequestHandler<ReVoteEstimateCommand, CommandResult>
{
    private readonly IStoryRepository _storyRepository;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessagesSender _messagesSender;

	public ReVoteEstimateCommandHandler(
        IStoryRepository storyRepository,
        SummaryByStoryBuilder summaryByStoryBuilder,
        IMessagesSender messagesSender)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
    }

    public async Task<CommandResult> Handle(ReVoteEstimateCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var story = await _storyRepository.Find(command.StoryId, token);
        if (story is null)
            throw new TeamAssistantUserException(Messages.Appraiser_StoryNotFound, command.StoryId);

        story.Reset(command.MessageContext.PersonId);
        
        await _storyRepository.Upsert(story, token);
        
        await _messagesSender.StoryChanged(story.TeamId);

        return CommandResult.Build(await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(story)));
    }
}