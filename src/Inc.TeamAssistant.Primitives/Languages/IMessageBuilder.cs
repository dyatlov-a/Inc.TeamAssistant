namespace Inc.TeamAssistant.Primitives.Languages;

public interface IMessageBuilder
{
    string Build(MessageId messageId, LanguageId languageId, params object[] values);
}