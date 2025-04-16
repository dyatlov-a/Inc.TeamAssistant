using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AttachStory;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AttachStory;

internal sealed class AttachStoryCommandHandler : IRequestHandler<AttachStoryCommand, CommandResult>
{
    private readonly IStoryRepository _repository;

    public AttachStoryCommandHandler(IStoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CommandResult> Handle(AttachStoryCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var story = await command.StoryId.Required(_repository.Find, token);
        
        await _repository.Upsert(story.SetExternalId(command.MessageId), token);

        return CommandResult.Empty;
    }
}