using MediatR;

namespace Inc.TeamAssistant.Appraiser.Notifications.Contracts;

public sealed class NotificationMessage
{
	public delegate IBaseRequest ResponseHandler(long chatId, string userName, int messageId);

	public string Text { get; }
    public IReadOnlyCollection<long>? TargetChatIds { get; }
    public IReadOnlyCollection<(long ChatId, int MessageId)>? TargetMessages { get; }
    public ResponseHandler? Handler { get; private set; }

    private NotificationMessage(
        IReadOnlyCollection<long>? targetChatIds,
        IReadOnlyCollection<(long UserId, int MessageId)>? targetMessages,
        string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        Text = text;
        TargetChatIds = targetChatIds;
        TargetMessages = targetMessages;
    }

    public NotificationMessage AddHandler(ResponseHandler handler)
    {
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));

        return this;
    }

    public static NotificationMessage Create(IReadOnlyCollection<long> targetChatIds, string text)
    {
        if (targetChatIds is null)
            throw new ArgumentNullException(nameof(targetChatIds));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        return new(targetChatIds, targetMessages: null, text);
    }

    public static NotificationMessage Create(long targetChatId, string text) => Create(new[] { targetChatId }, text);

    public static NotificationMessage Edit(
        IReadOnlyCollection<(long ChatId, int MessageId)> targetMessages,
        string text)
    {
        if (targetMessages is null)
            throw new ArgumentNullException(nameof(targetMessages));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        return new(targetChatIds: null, targetMessages, text);
    }
}