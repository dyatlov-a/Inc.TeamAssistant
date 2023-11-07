using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.DialogContinuations;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.JoinToAssessmentSession;

internal sealed class JoinToAssessmentSessionCommandHandler
    : IRequestHandler<JoinToAssessmentSessionCommand, CommandResult>
{
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;
    private readonly IMessageBuilder _messageBuilder;

    public JoinToAssessmentSessionCommandHandler(
        IDialogContinuation<ContinuationState> dialogContinuation,
        IMessageBuilder messageBuilder)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(JoinToAssessmentSessionCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        _dialogContinuation.TryBegin(command.AppraiserId.Value, ContinuationState.EnterSessionId);
        
        var message = await _messageBuilder.Build(Messages.EnterSessionId, command.LanguageId);

        return CommandResult.Build(NotificationMessage.Create(command.TargetChatId, message));
    }
}