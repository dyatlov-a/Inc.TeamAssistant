using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.Converters;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

internal sealed class AddStoryCommandHandler : IRequestHandler<AddStoryCommand, CommandResult>
{
    private readonly IStoryRepository _repository;
    private readonly SummaryByStoryBuilder _summaryBuilder;
    private readonly IMessagesSender _messagesSender;

    public AddStoryCommandHandler(
        IStoryRepository repository,
        SummaryByStoryBuilder summaryBuilder,
        IMessagesSender messagesSender)
	{
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _summaryBuilder = summaryBuilder ?? throw new ArgumentNullException(nameof(summaryBuilder));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
    }

    public async Task<CommandResult> Handle(AddStoryCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var targetTeam = command.MessageContext.EnsureTeam(command.TeamId);
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

        await _repository.Upsert(story, token);
        
        var notification = _summaryBuilder.Build(SummaryByStoryConverter.ConvertTo(story));
        await _messagesSender.StoryChanged(story.TeamId);
        return CommandResult.Build(notification);
    }
}