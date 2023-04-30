using System.Text;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class ChangeUserNotificationBuilder :
    INotificationBuilder<AllowUseNameResult>,
    INotificationBuilder<ExitFromAssessmentSessionResult>
{
	private readonly IMessagesSender _messagesSender;
	private readonly SummaryByStoryBuilder _summaryByStoryBuilder;
    private readonly IMessageBuilder _messageBuilder;

	public ChangeUserNotificationBuilder(
		IMessagesSender messagesSender,
		SummaryByStoryBuilder summaryByStoryBuilder,
        IMessageBuilder messageBuilder)
	{
		_messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
		_summaryByStoryBuilder =
			summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
	}

    public async IAsyncEnumerable<NotificationMessage> Build(AllowUseNameResult commandResult, long fromId)
    {
        if (commandResult == null)
            throw new ArgumentNullException(nameof(commandResult));

        if (commandResult.AssessmentSessionDetails is not null)
        {
            await _messagesSender.StoryChanged(commandResult.AssessmentSessionDetails.Id);

            yield return await CreateMessage(commandResult.AssessmentSessionDetails);
        }
    }

    public async IAsyncEnumerable<NotificationMessage> Build(ExitFromAssessmentSessionResult commandResult, long fromId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        var disconnectedFromSessionMessage = await _messageBuilder.Build(
            Messages.DisconnectedFromSession,
            commandResult.AssessmentSessionDetails.LanguageId,
            commandResult.AssessmentSessionDetails.Title);
        var appraiserDisconnectedFromSessionMessage = await _messageBuilder.Build(
            Messages.AppraiserDisconnectedFromSession,
            commandResult.AssessmentSessionDetails.LanguageId,
            commandResult.AppraiserName,
            commandResult.AssessmentSessionDetails.Title);

        await _messagesSender.StoryChanged(commandResult.AssessmentSessionDetails.Id);

        yield return NotificationMessage.Create(fromId, disconnectedFromSessionMessage);
        yield return NotificationMessage.Create(
            commandResult.ModeratorId.Value,
            appraiserDisconnectedFromSessionMessage);

        if (commandResult.AssessmentSessionDetails.Items.Any())
        {
            yield return await CreateMessage(commandResult.AssessmentSessionDetails);
        }
    }

    private async Task<NotificationMessage> CreateMessage(AssessmentSessionDetails assessmentSessionDetails)
    {
        if (assessmentSessionDetails is null)
            throw new ArgumentNullException(nameof(assessmentSessionDetails));
        
        var stringBuilder = new StringBuilder();
        await _summaryByStoryBuilder.AddStoryDetails(
            stringBuilder,
            Messages.NeedEstimate,
            assessmentSessionDetails.LanguageId,
            assessmentSessionDetails.Story);
        _summaryByStoryBuilder.AddEstimates(stringBuilder, assessmentSessionDetails.Items, estimateEnded: false);

        return _summaryByStoryBuilder.AddAssessments(NotificationMessage.Edit(
            new [] { (assessmentSessionDetails.ChatId, assessmentSessionDetails.Story.ExternalId) },
            stringBuilder.ToString()));
    }
}