using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

internal sealed class AddStoryCommandHandler : IRequestHandler<AddStoryCommand, CommandResult>
{
    private readonly IStoryRepository _storyRepository;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessagesSender _messagesSender;

    public AddStoryCommandHandler(
        IStoryRepository storyRepository,
        SummaryByStoryBuilder summaryByStoryBuilder,
        IMessagesSender messagesSender)
	{
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
    }

    public async Task<CommandResult> Handle(AddStoryCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var targetTeam = command.MessageContext.FindTeam(command.TeamId);
        if (targetTeam is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);

        var story = new Story(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            command.TeamId,
            Enum.Parse<StoryType>(command.StoryType),
            targetTeam.ChatId,
            command.MessageContext.Person.Id,
            command.MessageContext.LanguageId,
            command.Title);

        if(command.Links.Any())
            story.AddLink(command.Links.Single());

        foreach (var teammate in command.Teammates)
            story.AddStoryForEstimate(new StoryForEstimate(
                Guid.NewGuid(),
                story.Id,
                teammate.Id,
                teammate.DisplayName));

        await _storyRepository.Upsert(story, token);
        
        await _messagesSender.StoryChanged(story.TeamId);

        return CommandResult.Build(await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(story)));
    }
}