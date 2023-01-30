using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Common.Messages.Client;

public sealed class MessageBuilder : IMessageBuilder
{
    private readonly IMessageService _messageService;

    public MessageBuilder(IMessageService messageService)
    {
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
    }

    public async Task<string> Build(MessageId messageId, LanguageId languageId, params object[] values)
    {
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));
        if (values is null)
            throw new ArgumentNullException(nameof(values));

        var resources = await _messageService.GetAll();

        if (resources.State == ServiceResultState.Success
            && resources.Result[languageId.Value].TryGetValue(messageId.Value, out var message))
            return values.Any() ? string.Format(message, values) : message;

        throw new ApplicationException($"Not supported message with id {messageId.Value}.");
    }
}