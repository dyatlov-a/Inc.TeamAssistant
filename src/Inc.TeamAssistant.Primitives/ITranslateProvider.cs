namespace Inc.TeamAssistant.Primitives;

public interface ITranslateProvider
{
    Task<string> Get(MessageId messageId, LanguageId languageId, params object[] values);
}