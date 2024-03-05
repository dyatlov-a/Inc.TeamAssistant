namespace Inc.TeamAssistant.Primitives.Exceptions;

public class TeamAssistantUserException : TeamAssistantException
{
    public MessageId MessageId { get; }
    public object[] Values { get; }

    public TeamAssistantUserException(MessageId messageId, params object[] values)
        : base(messageId.Value)
    {
        MessageId = messageId;
        Values = values;
    }
}