using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

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

        throw new ApplicationException($"Not supported message with id {messageId.Value}.");
    }
}