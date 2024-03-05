namespace Inc.TeamAssistant.Primitives;

public interface IMessageBuilder
{
    Task<string> Build(MessageId messageId, LanguageId languageId, params object[] values);
}