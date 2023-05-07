using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Notifications.Services;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class AllowUseNameNotificationBuilder : INotificationBuilder<AllowUseNameResult>
{
    private readonly IMessagesSender _messagesSender;
    private readonly SummaryByStoryBuilder _summaryByStoryBuilder;

    public AllowUseNameNotificationBuilder(IMessagesSender messagesSender, SummaryByStoryBuilder summaryByStoryBuilder)
    {
        _messagesSender = messagesSender ?? throw new ArgumentNullException(nameof(messagesSender));
        _summaryByStoryBuilder = summaryByStoryBuilder ?? throw new ArgumentNullException(nameof(summaryByStoryBuilder));
    }

    public async IAsyncEnumerable<NotificationMessage> Build(AllowUseNameResult commandResult, long fromId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        if (commandResult is { InProgress: true, SummaryByStory: not null })
        {
            await _messagesSender.StoryChanged(commandResult.SummaryByStory.AssessmentSessionId);

            yield return await _summaryByStoryBuilder.Build(commandResult.SummaryByStory);
        }
    }
}