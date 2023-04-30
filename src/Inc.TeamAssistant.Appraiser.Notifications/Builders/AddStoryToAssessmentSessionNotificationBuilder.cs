using System.Text;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

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
		AddStoryToAssessmentSessionResult commandToAssessmentSessionResult,
		long fromId)
	{
		if (commandToAssessmentSessionResult is null)
			throw new ArgumentNullException(nameof(commandToAssessmentSessionResult));

		await _messagesSender.StoryChanged(commandToAssessmentSessionResult.AssessmentSessionId);
		
		var stringBuilder = new StringBuilder();

        await _summaryByStoryBuilder.AddStoryDetails(
	        stringBuilder,
	        Messages.NeedEstimate,
	        commandToAssessmentSessionResult.AssessmentSessionLanguageId,
	        commandToAssessmentSessionResult.Story);
        _summaryByStoryBuilder.AddEstimates(stringBuilder, commandToAssessmentSessionResult.Items, estimateEnded: false);

        yield return _summaryByStoryBuilder.AddAssessments(NotificationMessage
			.Create(commandToAssessmentSessionResult.ChatId, stringBuilder.ToString())
			.AddHandler((_, uName, mId) => AddStoryForEstimate(
				commandToAssessmentSessionResult.AssessmentSessionId,
				uName,
				mId)));
	}

	private IBaseRequest AddStoryForEstimate(
        AssessmentSessionId assessmentSessionId,
        string userName,
		int messageId)
	{
        if (assessmentSessionId is null)
            throw new ArgumentNullException(nameof(assessmentSessionId));
        if (string.IsNullOrWhiteSpace(userName))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(userName));

        return new AddStoryForEstimateCommand(assessmentSessionId, userName, messageId);
	}
}