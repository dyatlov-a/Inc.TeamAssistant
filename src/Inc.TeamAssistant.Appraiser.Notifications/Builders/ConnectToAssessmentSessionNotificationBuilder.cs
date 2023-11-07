using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ConnectToAssessmentSessionNotificationBuilder
    : INotificationBuilder<ConnectToAssessmentSessionResult>
{
    private readonly IMessagesSender _messagesSender;
    private readonly IMessageBuilder _messageBuilder;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;

    public ConnectToAssessmentSessionNotificationBuilder(
	    IMessagesSender messagesSender,
	    IMessageBuilder messageBuilder,
	    SummaryByStoryBuilder summaryByStoryBuilder)
    {
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
    }

    public async IAsyncEnumerable<NotificationMessage> Build(
	    ConnectToAssessmentSessionResult commandResult,
	    long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));
		
        var connectedSuccessMessage = await _messageBuilder.Build(
            Messages.ConnectedSuccess,
            commandResult.SummaryByStory.AssessmentSessionLanguageId,
            commandResult.AssessmentSessionTitle);
        yield return NotificationMessage.Create(fromId, connectedSuccessMessage);

        if (commandResult.IsModerator)
        {
	        var appraiserAddedMessage = await _messageBuilder.Build(
		        Messages.AppraiserAdded,
		        commandResult.SummaryByStory.AssessmentSessionLanguageId,
		        commandResult.AppraiserName,
		        commandResult.AssessmentSessionTitle);
	        yield return NotificationMessage.Create(
		        commandResult.ModeratorId.Value,
		        appraiserAddedMessage);
        }

        if (commandResult.InProgress)
        {
	        await _messagesSender.StoryChanged(commandResult.SummaryByStory.AssessmentSessionId);
	        
	        yield return await _summaryByStoryBuilder.Build(commandResult.SummaryByStory);
        }
	}
}