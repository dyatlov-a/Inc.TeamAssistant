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

    public Task<ExitFromAssessmentSessionResult> Handle(
        ExitFromAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository
            .Find(command.AppraiserId)
            .EnsureForAppraiser(command.AppraiserName);

		assessmentSession.Disconnect(command.AppraiserId);

        var result = new ExitFromAssessmentSessionResult(
            assessmentSession.Moderator.Id,
            command.AppraiserName,
            assessmentSession.Title,
            assessmentSession.InProgress(),
            SummaryByStoryConverter.ConvertTo(assessmentSession));

        return Task.FromResult(result);
    }
}