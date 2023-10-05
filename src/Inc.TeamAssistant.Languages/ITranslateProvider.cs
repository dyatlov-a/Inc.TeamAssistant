using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Languages;

public interface ITranslateProvider
{
    Task<string> Get(MessageId messageId, LanguageId languageId, params object[] values);
}