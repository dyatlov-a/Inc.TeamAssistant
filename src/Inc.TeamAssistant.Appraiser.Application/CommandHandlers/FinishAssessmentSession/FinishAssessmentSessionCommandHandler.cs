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

	public FinishAssessmentSessionCommandHandler(IAssessmentSessionRepository repository, IAssessmentSessionMetrics metrics)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
	}

    public Task<FinishAssessmentSessionResult> Handle(FinishAssessmentSessionCommand sessionCommand, CancellationToken cancellationToken)
    {
        if (sessionCommand is null)
            throw new ArgumentNullException(nameof(sessionCommand));

        var assessmentSession = _repository.Find(sessionCommand.ModeratorId).EnsureForModerator(sessionCommand.ModeratorName);

		if (assessmentSession.Moderator.Id != sessionCommand.ModeratorId)
			throw new AppraiserUserException(Messages.EndAssessmentWithError);

        _repository.Remove(assessmentSession);
        _metrics.Ended();

		var appraiserIds = assessmentSession.Participants.Select(a => a.Id).ToArray();
        var result = new FinishAssessmentSessionResult(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            assessmentSession.Title,
            appraiserIds);

        return Task.FromResult(result);
    }
}