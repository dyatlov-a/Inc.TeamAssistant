using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class TranslateProvider : ITranslateProvider
{
    private readonly IMessageBuilder _messageBuilder;

    public TranslateProvider(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<string> Get(MessageId messageId, LanguageId languageId, params object[] values)
    {
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        return await _messageBuilder.Build(messageId, languageId, values);
    }
}