using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class StartStorySelectionNotificationBuilder : INotificationBuilder<StartStorySelectionResult>
{
    private readonly IMessageBuilder _messageBuilder;

    public StartStorySelectionNotificationBuilder(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async IAsyncEnumerable<NotificationMessage> Build(StartStorySelectionResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

        var message = await _messageBuilder.Build(
            Messages.EnterStoryName,
            new(commandResult.AssessmentSessionLanguageId));

		yield return NotificationMessage.Create(fromId, message);
	}
}