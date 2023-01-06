using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToDashboard;

internal sealed class ConnectToDashboardCommandHandler
	: IRequestHandler<ConnectToDashboardCommand, ConnectToDashboardResult>
{
	private readonly IAssessmentSessionRepository _repository;

	public ConnectToDashboardCommandHandler(IAssessmentSessionRepository repository)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
	}

	public Task<ConnectToDashboardResult> Handle(ConnectToDashboardCommand command, CancellationToken cancellationToken)
	{
		if (command is null)
			throw new ArgumentNullException(nameof(command));

		var assessmentSession = _repository
			.Find(command.AppraiserId)
			.EnsureForAppraiser(command.AppraiserName);

		return Task.FromResult(new ConnectToDashboardResult(
            assessmentSession.Id,
            assessmentSession.LanguageId,
            assessmentSession.Title));
	}
}