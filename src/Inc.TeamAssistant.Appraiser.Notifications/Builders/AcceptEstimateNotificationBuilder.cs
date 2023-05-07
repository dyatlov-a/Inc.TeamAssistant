using Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class AcceptEstimateNotificationBuilder : INotificationBuilder<AcceptEstimateResult>
{
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
	private readonly IMessagesSender _messagesSender;

	public AcceptEstimateNotificationBuilder(SummaryByStoryBuilder summaryByStoryBuilder, IMessagesSender messagesSender)
	{
		_summaryByStoryBuilder =
			summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
	}

	public async IAsyncEnumerable<NotificationMessage> Build(AcceptEstimateResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		await _messagesSender.StoryChanged(commandResult.SummaryByStory.AssessmentSessionId);

		yield return await _summaryByStoryBuilder.Build(commandResult.SummaryByStory);
	}
}