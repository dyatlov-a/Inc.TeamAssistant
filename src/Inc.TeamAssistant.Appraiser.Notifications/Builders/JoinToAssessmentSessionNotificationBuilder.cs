using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class JoinToAssessmentSessionNotificationBuilder : INotificationBuilder<JoinToAssessmentSessionResult>
{
    private readonly IMessageBuilder _messageBuilder;

    public JoinToAssessmentSessionNotificationBuilder(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async IAsyncEnumerable<NotificationMessage> Build(JoinToAssessmentSessionResult commandResult, long fromId)
    {
        if (commandResult is null)
            throw new ArgumentNullException(nameof(commandResult));

        var message = await _messageBuilder.Build(Messages.EnterSessionId, commandResult.LanguageId);

        yield return NotificationMessage.Create(fromId, message);
    }
}