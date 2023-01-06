using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.CheckIn.All.Contracts;

public interface ITranslateProvider
{
    Task<string> Get(MessageId messageId, LanguageId languageId, params object[] values);
}