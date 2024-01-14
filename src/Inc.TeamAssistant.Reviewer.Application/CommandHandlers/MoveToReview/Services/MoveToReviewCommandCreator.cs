using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Services;

internal sealed class MoveToReviewCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    
    public string Command => "/need_review";

    public MoveToReviewCommandCreator(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    public async Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        CurrentTeamContext? teamContext,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.Text.StartsWith("/"))
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "Command",
                    await _messageBuilder.Build(Messages.Reviewer_TaskTitleIsEmpty, messageContext.LanguageId))
            });
        if (teamContext is null)
            throw new ApplicationException("Team was not selected.");
            
        return new MoveToReviewCommand(messageContext, teamContext.TeamId, messageContext.Text);
    }
}