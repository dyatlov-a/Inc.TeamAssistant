using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Services;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory;

internal sealed class AddStoryCommandHandler : IRequestHandler<AddStoryCommand, CommandResult>
{
    private readonly IStoryRepository _storyRepository;
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessagesSender _messagesSender;

    public AddStoryCommandHandler(
        IStoryRepository storyRepository,
        IDialogContinuation<BotCommandStage> dialogContinuation,
        SummaryByStoryBuilder summaryByStoryBuilder,
        IMessagesSender messagesSender)
	{
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
    }

    public async Task<CommandResult> Handle(AddStoryCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var story = new Story(
            command.TeamId,
            command.MessageContext.ChatId,
            command.MessageContext.PersonId,
            command.MessageContext.LanguageId,
            command.Title);

        foreach (var link in command.Links)
            story.AddLink(link);

        foreach (var teammate in command.Teammates)
            story.AddStoryForEstimate(new StoryForEstimate(story.Id, teammate.PersonId, teammate.PersonDisplayName));

        await _storyRepository.Upsert(story, token);
        
        var dialogState = _dialogContinuation.TryEnd(command.MessageContext.PersonId, BotCommandStage.EnterText);
        var notifications = new List<NotificationMessage>
        {
            await _summaryByStoryBuilder.Build(SummaryByStoryConverter.ConvertTo(story))
        };
        
        if (dialogState is not null)
        {
            if (command.MessageContext.Shared)
                dialogState.TryAttachMessage(new ChatMessage(
                    command.MessageContext.ChatId,
                    command.MessageContext.MessageId));
            
            if (dialogState.ChatMessages.Any())
                notifications.Add(NotificationMessage.Delete(dialogState.ChatMessages));
        }
        
        await _messagesSender.StoryChanged(story.TeamId);

        return CommandResult.Build(notifications.ToArray());
    }
}