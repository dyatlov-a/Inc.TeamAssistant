using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.DialogContinuations;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment;

internal sealed class ActivateAssessmentCommandHandler : IRequestHandler<ActivateAssessmentCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;
    private readonly IAssessmentSessionMetrics _metrics;
    private readonly ILinkBuilder _linkBuilder;
    private readonly IMessageBuilder _messageBuilder;

	public ActivateAssessmentCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation<ContinuationState> dialogContinuation,
        IAssessmentSessionMetrics metrics,
        ILinkBuilder linkBuilder,
        IMessageBuilder messageBuilder)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
	}

    public async Task<CommandResult> Handle(ActivateAssessmentCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		assessmentSession.Activate(command.ModeratorId, command.Title);
        _dialogContinuation.End(command.ModeratorId.Value, ContinuationState.EnterTitle);

        _metrics.Started();
        
        var connectToDashboardMessage = await _messageBuilder.Build(
	        Messages.ConnectToDashboard,
	        assessmentSession.LanguageId,
	        assessmentSession.Title,
	        _linkBuilder.BuildLinkForDashboard(
		        assessmentSession.Id,
		        assessmentSession.LanguageId));
        
        var connectToSessionMessage = await _messageBuilder.Build(
	        Messages.ConnectToSession,
	        assessmentSession.LanguageId,
	        assessmentSession.Title,
	        _linkBuilder.BuildLinkForConnect(assessmentSession.Id),
	        assessmentSession.Id.Value.ToString("N"));

        return CommandResult.Build(
	        NotificationMessage.Create(command.TargetChatId, connectToDashboardMessage),
	        NotificationMessage.Create(command.TargetChatId, connectToSessionMessage));
    }
}