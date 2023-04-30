using Inc.TeamAssistant.Appraiser.Model.Commands.SetEstimateForStory;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class SetEstimateForStoryNotificationBuilder : INotificationBuilder<SetEstimateForStoryResult>
{
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
	private readonly IMessagesSender _messagesSender;

	public SetEstimateForStoryNotificationBuilder(
		SummaryByStoryBuilder summaryByStoryBuilder,
		IMessagesSender messagesSender)
	{
		_summaryByStoryBuilder =
			summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
	}

	public async IAsyncEnumerable<NotificationMessage> Build(SetEstimateForStoryResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

		await _messagesSender.StoryChanged(commandResult.AssessmentSessionId);

		yield return await _summaryByStoryBuilder.Build(
            commandResult.AssessmentSessionLanguageId,
            commandResult.Summary,
            commandResult.EstimateEnded);
		
		if (commandResult.EstimateEnded)
			yield return await _summaryByStoryBuilder.Build(
				commandResult.AssessmentSessionLanguageId,
				commandResult.Summary,
				estimateEnded: false);
	}
}