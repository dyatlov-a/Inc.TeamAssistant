using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ExitFromAssessmentSession;

internal sealed class ExitFromAssessmentSessionCommandHandler
    : IRequestHandler<ExitFromAssessmentSessionCommand, ExitFromAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _repository;

	public ExitFromAssessmentSessionCommandHandler(IAssessmentSessionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

    public Task<ExitFromAssessmentSessionResult> Handle(ExitFromAssessmentSessionCommand sessionCommand, CancellationToken cancellationToken)
    {
        if (sessionCommand is null)
            throw new ArgumentNullException(nameof(sessionCommand));

        var assessmentSession = _repository.Find(sessionCommand.AppraiserId).EnsureForAppraiser(sessionCommand.AppraiserName);

		assessmentSession.Disconnect(sessionCommand.AppraiserId);

        var result = new ExitFromAssessmentSessionResult(
            AssessmentSessionConverter.ConvertTo(assessmentSession),
            assessmentSession.Moderator.Id,
            sessionCommand.AppraiserName);

        return Task.FromResult(result);
    }
}