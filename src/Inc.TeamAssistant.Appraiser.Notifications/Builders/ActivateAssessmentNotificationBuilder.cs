using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ActivateAssessmentNotificationBuilder
	: INotificationBuilder<ActivateAssessmentResult>
{
    private readonly ILinkBuilder _linkBuilder;
    private readonly IMessageBuilder _messageBuilder;

	public ActivateAssessmentNotificationBuilder(ILinkBuilder linkBuilder, IMessageBuilder messageBuilder)
    {
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

	public async IAsyncEnumerable<NotificationMessage> Build(ActivateAssessmentResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

        var linkForDashboard = _linkBuilder.BuildLinkForDashboard(
            commandResult.AssessmentSessionId,
            commandResult.AssessmentSessionLanguageId);
        var connectToDashboardMessage = await _messageBuilder.Build(
            Messages.ConnectToDashboard,
            commandResult.AssessmentSessionLanguageId,
            commandResult.AssessmentSessionTitle,
            linkForDashboard);
        
        var linkForConnect = _linkBuilder.BuildLinkForConnect(commandResult.AssessmentSessionId);
        var connectToSessionMessage = await _messageBuilder.Build(
            Messages.ConnectToSession,
            commandResult.AssessmentSessionLanguageId,
            commandResult.AssessmentSessionTitle,
            linkForConnect,
            commandResult.AssessmentSessionId.Value.ToString("N"));

        yield return NotificationMessage.Create(fromId, connectToDashboardMessage);
        yield return NotificationMessage.Create(fromId, connectToSessionMessage);
    }
}