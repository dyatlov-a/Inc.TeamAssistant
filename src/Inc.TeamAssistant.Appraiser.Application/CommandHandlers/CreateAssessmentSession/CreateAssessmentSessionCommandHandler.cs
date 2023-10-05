using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.DialogContinuations;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;

internal sealed class CreateAssessmentSessionCommandHandler
    : IRequestHandler<CreateAssessmentSessionCommand, CreateAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;
    private readonly IAssessmentSessionMetrics _metrics;

    public CreateAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation<ContinuationState> dialogContinuation,
        IAssessmentSessionMetrics metrics)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

	public Task<CreateAssessmentSessionResult> Handle(
        CreateAssessmentSessionCommand command,
        CancellationToken cancellationToken)
	{
		if (command is null)
			throw new ArgumentNullException(nameof(command));

        var existsSession = _repository.Find(command.ModeratorId);
        var isNotExists = existsSession is null;

        if (isNotExists)
        {
            var moderator = new Participant(command.ModeratorId, command.ModeratorName);
            var assessmentSession = new AssessmentSession(command.ChatId, moderator, command.LanguageId);

            _dialogContinuation.TryBegin(command.ModeratorId.Value, ContinuationState.EnterTitle);
            _repository.Add(assessmentSession);

            _metrics.Created();
        }

        return Task.FromResult(new CreateAssessmentSessionResult(command.LanguageId, IsCreated: isNotExists));
    }
}