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

        var linkForConnect = _linkBuilder.BuildLinkForConnect(commandResult.AssessmentSessionId);
        var message = await _messageBuilder.Build(
            Messages.ConnectToSession,
            commandResult.AssessmentSessionLanguageId,
            commandResult.Title,
            linkForConnect,
            commandResult.AssessmentSessionId.Value.ToString("N"));

        yield return NotificationMessage.Create(fromId, message);
    }
}