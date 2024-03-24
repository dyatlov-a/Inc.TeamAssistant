namespace Inc.TeamAssistant.Primitives.Languages;

public interface ITranslateProvider
{
    Task<string> Get(MessageId messageId, LanguageId languageId, params object[] values);
}