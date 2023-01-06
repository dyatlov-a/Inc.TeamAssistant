using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToDashboard;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ConnectToDashboardNotificationBuilder : INotificationBuilder<ConnectToDashboardResult>
{
	private readonly ILinkBuilder _linkBuilder;
    private readonly IMessageBuilder _messageBuilder;

	public ConnectToDashboardNotificationBuilder(ILinkBuilder linkBuilder, IMessageBuilder messageBuilder)
    {
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

	public async IAsyncEnumerable<NotificationMessage> Build(ConnectToDashboardResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		var linkForDashboard = _linkBuilder.BuildLinkForDashboard(
            commandResult.AssessmentSessionId,
            commandResult.AssessmentSessionLanguageId);
        var message = await _messageBuilder.Build(
            Messages.ConnectToDashboard,
            commandResult.AssessmentSessionLanguageId,
            commandResult.AssessmentSessionTitle,
            linkForDashboard);

		yield return NotificationMessage.Create(fromId, message);
	}
}