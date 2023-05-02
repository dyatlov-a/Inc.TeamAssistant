using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;

namespace Inc.TeamAssistant.Appraiser.Notifications.Builders;

internal sealed class CreateAssessmentSessionNotificationBuilder : INotificationBuilder<CreateAssessmentSessionResult>
{
    private readonly IEnumerable<LanguageContext> _languageContextList;
    private readonly IMessageBuilder _messageBuilder;

    public CreateAssessmentSessionNotificationBuilder(
        IEnumerable<LanguageContext> languageContextList,
        IMessageBuilder messageBuilder)
    {
        _languageContextList = languageContextList ?? throw new ArgumentNullException(nameof(languageContextList));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

	public async IAsyncEnumerable<NotificationMessage> Build(CreateAssessmentSessionResult commandResult, long fromId)
	{
		if (commandResult is null)
			throw new ArgumentNullException(nameof(commandResult));

        var otherLanguages = _languageContextList.Where(c => c.LanguageId != commandResult.LanguageId);
        var setLanguageCommands = string.Join(' ', otherLanguages.Select(l => l.Command));

        if (commandResult.IsCreated)
        {
            var enterSessionNameMessage = await _messageBuilder.Build(
                Messages.EnterSessionName,
                commandResult.LanguageId,
                setLanguageCommands);

            yield return NotificationMessage.Create(fromId, enterSessionNameMessage);
        }
        else
        {
            var createAssessmentSessionFailedMessage = await _messageBuilder.Build(
                Messages.CreateAssessmentSessionFailed,
                commandResult.LanguageId);

            yield return NotificationMessage.Create(fromId, createAssessmentSessionFailedMessage);
        }
    }
}