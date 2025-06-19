using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives.Exceptions;

public sealed class TeamAssistantUserException : TeamAssistantException
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