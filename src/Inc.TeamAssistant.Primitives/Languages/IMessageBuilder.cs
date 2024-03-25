namespace Inc.TeamAssistant.Primitives.Languages;

public interface IMessageBuilder
{
    Task<string> Build(MessageId messageId, LanguageId languageId, params object[] values);
}