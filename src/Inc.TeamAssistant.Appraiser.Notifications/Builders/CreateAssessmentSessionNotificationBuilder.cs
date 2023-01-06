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

	public async IAsyncEnumerable<NotificationMessage> Build(CreateAssessmentSessionResult commandSessionResult, long fromId)
	{
		if (commandSessionResult is null)
			throw new ArgumentNullException(nameof(commandSessionResult));

        var otherLanguages = _languageContextList.Where(c => c.LanguageId != commandSessionResult.LanguageId);
        var setLanguageCommands = string.Join(' ', otherLanguages.Select(l => l.Command));

        if (commandSessionResult.IsCreated)
        {
            var enterSessionNameMessage = await _messageBuilder.Build(
                Messages.EnterSessionName,
                commandSessionResult.LanguageId,
                setLanguageCommands);

            yield return NotificationMessage.Create(fromId, enterSessionNameMessage);
        }
        else
        {
            var createAssessmentSessionFailedMessage = await _messageBuilder.Build(
                Messages.CreateAssessmentSessionFailed,
                commandSessionResult.LanguageId);

            yield return NotificationMessage.Create(fromId, createAssessmentSessionFailedMessage);
        }
    }
}