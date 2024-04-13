namespace Inc.TeamAssistant.Primitives.Notifications;

public sealed record ChatMessage(long ChatId, int MessageId)
{
    public bool OnlyChat => MessageId == 0;
}