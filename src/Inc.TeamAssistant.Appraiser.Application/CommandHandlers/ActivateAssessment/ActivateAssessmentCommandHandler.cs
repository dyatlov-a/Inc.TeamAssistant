using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment;

internal sealed class ActivateAssessmentCommandHandler
	: IRequestHandler<ActivateAssessmentCommand, ActivateAssessmentResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation _dialogContinuation;
    private readonly IAssessmentSessionMetrics _metrics;

	public ActivateAssessmentCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation dialogContinuation,
        IAssessmentSessionMetrics metrics)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
	}

    public Task<ActivateAssessmentResult> Handle(ActivateAssessmentCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		assessmentSession.Activate(command.ModeratorId, command.Title);
        _dialogContinuation.End(command.ModeratorId, ContinuationState.EnterTitle);

        _metrics.Started();

		return Task.FromResult(new ActivateAssessmentResult(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            assessmentSession.Title));
    }
}