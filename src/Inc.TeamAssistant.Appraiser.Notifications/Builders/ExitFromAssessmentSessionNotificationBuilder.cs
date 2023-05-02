using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ExitFromAssessmentSessionNotificationBuilder
    : INotificationBuilder<ExitFromAssessmentSessionResult>
{
	private readonly IMessagesSender _messagesSender;
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessageBuilder _messageBuilder;

	public ExitFromAssessmentSessionNotificationBuilder(
		IMessagesSender messagesSender,
		SummaryByStoryBuilder summaryByStoryBuilder,
        IMessageBuilder messageBuilder)
	{
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
		_summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
	}

    public async IAsyncEnumerable<NotificationMessage> Build(ExitFromAssessmentSessionResult commandResult, long fromId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        var disconnectedFromSessionMessage = await _messageBuilder.Build(
            Messages.DisconnectedFromSession,
            commandResult.SummaryByStory.AssessmentSessionLanguageId,
            commandResult.AssessmentSessionTitle);
        var appraiserDisconnectedFromSessionMessage = await _messageBuilder.Build(
            Messages.AppraiserDisconnectedFromSession,
            commandResult.SummaryByStory.AssessmentSessionLanguageId,
            commandResult.AppraiserName,
            commandResult.AssessmentSessionTitle);

        yield return NotificationMessage.Create(fromId, disconnectedFromSessionMessage);
        yield return NotificationMessage.Create(
            commandResult.ModeratorId.Value,
            appraiserDisconnectedFromSessionMessage);

        if (commandResult.InProgress)
        {
            await _messagesSender.StoryChanged(commandResult.SummaryByStory.AssessmentSessionId);
            
            yield return await _summaryByStoryBuilder.Build(commandResult.SummaryByStory);
        }
    }
}