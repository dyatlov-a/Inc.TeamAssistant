using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class FinishAssessmentSessionNotificationBuilder : INotificationBuilder<FinishAssessmentSessionResult>
{
	private readonly IMessagesSender _messagesSender;
    private readonly IMessageBuilder _messageBuilder;

	public FinishAssessmentSessionNotificationBuilder(IMessagesSender messagesSender, IMessageBuilder messageBuilder)
    {
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

	public async IAsyncEnumerable<NotificationMessage> Build(FinishAssessmentSessionResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		await _messagesSender.StoryChanged(commandResult.AssessmentSessionId);

        var targets = commandResult.AppraiserIds.Select(a => a.Value).Append(fromId).Distinct().ToArray();
        var message = await _messageBuilder.Build(
            Messages.SessionEnded,
            commandResult.AssessmentSessionLanguageId,
            commandResult.AssessmentSessionTitle);

		yield return NotificationMessage.Create(targets, message);
	}
}