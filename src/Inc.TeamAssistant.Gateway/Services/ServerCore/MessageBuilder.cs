using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class MessageBuilder : IMessageBuilder
{
    private readonly IMessageProvider _messageProvider;

    public MessageBuilder(IMessageProvider messageProvider)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
    }

    public string Build(MessageId messageId, LanguageId languageId, params object[] values)
    {
        ArgumentNullException.ThrowIfNull(messageId);
        ArgumentNullException.ThrowIfNull(languageId);
        ArgumentNullException.ThrowIfNull(values);
        
        if (_messageProvider.Data[languageId.Value].TryGetValue(messageId.Value, out var message))
            return values.Any() ? string.Format(message, values) : message;

        throw new TeamAssistantException($"Not supported message with id {messageId.Value}.");
    }
}