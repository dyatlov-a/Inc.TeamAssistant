using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands.Begin;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;

internal sealed class BeginMoveToReviewCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;
    private readonly ITeamAccessor _teamAccessor;
    
    public int Priority => 4;

    public BeginMoveToReviewCommandCreator(
        IMessageBuilder messageBuilder,
        IDialogContinuation<BotCommandStage> dialogContinuation, ITeamAccessor teamAccessor)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }
    
    public async Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        var dialogState = _dialogContinuation.Find(messageContext.PersonId);
        if (messageContext.Cmd.Equals(CommandList.MoveToReview, StringComparison.InvariantCultureIgnoreCase))
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
                    var teamId = messageContext.Teams[0].Id;
                    await ValidateTeam(teamId, messageContext.LanguageId, token);
                    
                    _dialogContinuation.TryBegin(messageContext.PersonId, BotCommandStage.EnterText, out dialogState);
                    dialogState.AddItem(CommandList.MoveToReview).AddItem(teamId.ToString());
                
                    var notification = NotificationMessage.Create(
                        messageContext.ChatId,
                        await _messageBuilder.Build(Messages.Reviewer_EnterRequestForReview, messageContext.LanguageId));
                    return new BeginCommand(messageContext, BotCommandStage.EnterText, CommandList.MoveToReview, notification);
                }
                else
                {
                    var notification = NotificationMessage.Create(
                        messageContext.ChatId,
                        await _messageBuilder.Build(Messages.Connector_SelectTeam, messageContext.LanguageId));
            
                    foreach (var team in messageContext.Teams)
                        notification.WithButton(new Button(team.Name, $"/{team.Id:N}"));
            
                    return new BeginCommand(messageContext, BotCommandStage.SelectTeam, CommandList.MoveToReview, notification);
                }
            }

            if (dialogState.ContinuationState == BotCommandStage.SelectTeam)
            {
                var teamId = Guid.Parse(messageContext.Text.TrimStart('/'));
                await ValidateTeam(teamId, messageContext.LanguageId, token);
                
                var notification = NotificationMessage.Create(
                    messageContext.ChatId,
                    await _messageBuilder.Build(Messages.Reviewer_EnterRequestForReview, messageContext.LanguageId));
                
                dialogState.AddItem(teamId.ToString());
                return new BeginCommand(messageContext, BotCommandStage.EnterText, CommandList.MoveToReview, notification);
            }
        }

        return null;
    }

    private async Task ValidateTeam(Guid teamId, LanguageId languageId, CancellationToken token)
    {
        var teammates = await _teamAccessor.GetTeammates(teamId, token);
        if (teammates.Count < 2)
            throw new ValidationException(new[]
            {
                new ValidationFailure("Teams", await _messageBuilder.Build(Messages.Reviewer_TeamMinError, languageId))
            });
    }
}