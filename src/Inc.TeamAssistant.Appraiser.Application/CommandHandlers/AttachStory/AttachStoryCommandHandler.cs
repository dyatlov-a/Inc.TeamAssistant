using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AttachStory;

internal sealed class AttachStoryCommandHandler : IRequestHandler<AttachStoryCommand, CommandResult>
{
    private readonly IStoryRepository _storyRepository;

    public AttachStoryCommandHandler(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
    }

    public async Task<CommandResult> Handle(AttachStoryCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var story = await _storyRepository.Find(command.StoryId, token);
        if (story is null)
            throw new TeamAssistantUserException(Messages.Appraiser_StoryNotFound, command.StoryId);
        
        story.SetExternalId(command.MessageId);
        
        await _storyRepository.Upsert(story, token);

        return CommandResult.Empty;
    }
}