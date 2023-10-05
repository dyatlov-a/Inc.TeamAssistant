using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.DialogContinuations;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.JoinToAssessmentSession;

internal sealed class JoinToAssessmentSessionCommandHandler
    : IRequestHandler<JoinToAssessmentSessionCommand, JoinToAssessmentSessionResult>
{
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;

    public JoinToAssessmentSessionCommandHandler(IDialogContinuation<ContinuationState> dialogContinuation)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<JoinToAssessmentSessionResult> Handle(
        JoinToAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        _dialogContinuation.TryBegin(command.AppraiserId.Value, ContinuationState.EnterSessionId);

        return Task.FromResult<JoinToAssessmentSessionResult>(new(command.LanguageId));
    }
}