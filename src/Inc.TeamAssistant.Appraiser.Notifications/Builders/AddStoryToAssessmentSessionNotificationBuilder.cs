using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class AddStoryToAssessmentSessionNotificationBuilder
    : INotificationBuilder<AddStoryToAssessmentSessionResult>
{
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
	private readonly IMessagesSender _messagesSender;

	public AddStoryToAssessmentSessionNotificationBuilder(
		SummaryByStoryBuilder summaryByStoryBuilder,
		IMessagesSender messagesSender)
	{
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
    }

	public async IAsyncEnumerable<NotificationMessage> Build(
		AddStoryToAssessmentSessionResult commandResult,
		long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		await _messagesSender.StoryChanged(commandResult.SummaryByStory.AssessmentSessionId);

		yield return await _summaryByStoryBuilder.Build(commandResult.SummaryByStory);
	}
}