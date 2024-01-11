using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;

internal sealed class MoveToReviewCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly IDialogContinuation<BotCommandStage> _dialogContinuation;
    
    private readonly BotCommandStage _commandStage = BotCommandStage.EnterText;

    public MoveToReviewCommandCreator(
        IDialogContinuation<BotCommandStage> dialogContinuation,
        IMessageBuilder messageBuilder)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public int Priority => 3;
    
    public async Task<IRequest<CommandResult>?> Create(MessageContext messageContext, CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        var dialogState = _dialogContinuation.Find(messageContext.PersonId);
        if (messageContext.Cmd.Equals(CommandList.MoveToReview, StringComparison.InvariantCultureIgnoreCase) &&
            messageContext.CurrentCommandStage == _commandStage &&
            dialogState is not null)
        {
            if (messageContext.Text.StartsWith("/"))
                throw new ValidationException(new[]
                {
                    new ValidationFailure(
                        "Command",
                        await _messageBuilder.Build(Messages.Reviewer_TaskTitleIsEmpty, messageContext.LanguageId))
                });
            
            var teamId = Guid.Parse(dialogState.Data[1]);
            return new MoveToReviewCommand(messageContext, teamId, messageContext.Text);
        }

        return null;
    }
}