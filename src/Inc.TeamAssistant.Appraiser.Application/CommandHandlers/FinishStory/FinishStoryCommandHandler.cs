using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishStory;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishStory;

internal sealed class FinishStoryCommandHandler : IRequestHandler<FinishStoryCommand, CommandResult>
{
    private readonly IStoryRepository _storyRepository;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;

    public FinishStoryCommandHandler(IStoryRepository storyRepository, SummaryByStoryBuilder summaryByStoryBuilder)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _summaryByStoryBuilder =
            summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
    }

    public async Task<CommandResult> Handle(FinishStoryCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var story = await _storyRepository.Find(command.StoryId, token);
        if (story is null)
            throw new TeamAssistantUserException(Messages.Appraiser_StoryNotFound, command.StoryId);
        
        story.MoveToFinish(command.MessageContext.Person.Id);
        
        await _storyRepository.Upsert(story, token);

        return CommandResult.Build(await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(story)));
    }
}