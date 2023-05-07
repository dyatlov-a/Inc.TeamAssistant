using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.JoinToAssessmentSession;

internal sealed class JoinToAssessmentSessionCommandHandler
    : IRequestHandler<JoinToAssessmentSessionCommand, JoinToAssessmentSessionResult>
{
    private readonly IDialogContinuation _dialogContinuation;

    public JoinToAssessmentSessionCommandHandler(IDialogContinuation dialogContinuation)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<JoinToAssessmentSessionResult> Handle(
        JoinToAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        _dialogContinuation.Begin(command.AppraiserId, ContinuationState.EnterSessionId);

        return Task.FromResult<JoinToAssessmentSessionResult>(new(command.LanguageId));
    }
}