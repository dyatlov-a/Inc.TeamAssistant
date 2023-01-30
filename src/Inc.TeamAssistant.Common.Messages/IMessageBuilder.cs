using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Common.Messages;

public interface IMessageBuilder
{
    Task<string> Build(MessageId messageId, LanguageId languageId, params object[] values);
}