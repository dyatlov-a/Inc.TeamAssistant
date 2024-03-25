using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class MessageBuilder : IMessageBuilder
{
    private readonly IMessageProvider _messageProvider;

    public MessageBuilder(IMessageProvider messageProvider)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
    }

    public async Task<string> Build(MessageId messageId, LanguageId languageId, params object[] values)
    {
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        var resources = await _messageProvider.Get();

        if (resources.State == ServiceResultState.Success
            && resources.Result[languageId.Value].TryGetValue(messageId.Value, out var message))
            return values.Any() ? string.Format(message, values) : message;

        throw new TeamAssistantException($"Not supported message with id {messageId.Value}.");
    }
}