using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class MessageBuilder : IMessageBuilder
{
    private readonly IMessageProvider _messageProvider;

    public MessageBuilder(IMessageProvider messageProvider)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
    }

    public async Task<string> Build(MessageId messageId, LanguageId languageId, params object[] values)
    {
        ArgumentNullException.ThrowIfNull(messageId);
        ArgumentNullException.ThrowIfNull(values);

        var resources = await _messageProvider.Get();

        if (resources[languageId.Value].TryGetValue(messageId.Value, out var message))
            return values.Any() ? string.Format(message, values) : message;

        throw new TeamAssistantException($"Not supported message with id {messageId.Value}.");
    }
}