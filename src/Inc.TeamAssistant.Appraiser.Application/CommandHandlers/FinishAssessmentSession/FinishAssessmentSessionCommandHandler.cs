using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishAssessmentSession;

internal sealed class FinishAssessmentSessionCommandHandler
    : IRequestHandler<FinishAssessmentSessionCommand, FinishAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IAssessmentSessionMetrics _metrics;

	public FinishAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IAssessmentSessionMetrics metrics)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
	}

    public Task<FinishAssessmentSessionResult> Handle(
        FinishAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		if (assessmentSession.Moderator.Id != command.ModeratorId)
			throw new AppraiserUserException(Messages.EndAssessmentWithError);

        _repository.Remove(assessmentSession);
        _metrics.Ended();

        return Task.FromResult<FinishAssessmentSessionResult>(new(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            assessmentSession.Title,
            assessmentSession.Participants.Select(a => a.Id).ToArray()));
    }
}