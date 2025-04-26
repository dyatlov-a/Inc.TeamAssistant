namespace Inc.TeamAssistant.Primitives.Notifications;

public sealed record ChatMessage(long ChatId, int MessageId, int? ReplyToMessageId = null)
{
    public static readonly ChatMessage Empty = new(ChatId: 0, MessageId: 0);
    
    public bool OnlyChat => MessageId == 0;
}