using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands.Begin;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Services;

internal sealed class BeginSelectTeamForAddStoryCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;
    
    private readonly string _command = "/add";

    public BeginSelectTeamForAddStoryCommandCreator(
        IMessageBuilder messageBuilder,
        IDialogContinuation<BotCommandStage> dialogContinuation)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public int Priority => 4;
    
    public async Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var dialogState = _dialogContinuation.Find(messageContext.PersonId);
        if (messageContext.Cmd.Equals(_command, StringComparison.InvariantCultureIgnoreCase))
        {
            if (dialogState is null)
            {
                if (!messageContext.Teams.Any())
                    throw new ValidationException(new[]
                    {
                        new ValidationFailure(
                            "Teams",
                            await _messageBuilder.Build(Messages.Connector_TeamNotFound, messageContext.LanguageId))
                    });
                if (messageContext.Teams.Count == 1)
                {
                    _dialogContinuation.TryBegin(messageContext.PersonId, BotCommandStage.EnterText, out dialogState);
                    dialogState?.AddItem(_command).AddItem(messageContext.Teams[0].Id.ToString());
                
                    var notification = NotificationMessage.Create(
                        messageContext.ChatId,
                        await _messageBuilder.Build(Messages.Appraiser_EnterStoryName, messageContext.LanguageId));
                    return new BeginCommand(messageContext, BotCommandStage.EnterText, _command, notification);
                }
                else
                {
                    var notification = NotificationMessage.Create(
                        messageContext.ChatId,
                        await _messageBuilder.Build(Messages.Connector_SelectTeam, messageContext.LanguageId));
            
                    foreach (var team in messageContext.Teams)
                        notification.WithButton(new Button(team.Name, $"/{team.Id:N}"));
            
                    return new BeginCommand(messageContext, BotCommandStage.SelectTeam, _command, notification);
                }
            }

            if (dialogState.ContinuationState == BotCommandStage.SelectTeam)
            {
                var notification = NotificationMessage.Create(
                    messageContext.ChatId,
                    await _messageBuilder.Build(Messages.Appraiser_EnterStoryName, messageContext.LanguageId));
                dialogState.AddItem(messageContext.Text.TrimStart('/'));
                return new BeginCommand(messageContext, BotCommandStage.EnterText, _command, notification);
            }
        }

        return null;
    }
}