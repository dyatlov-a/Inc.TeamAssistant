using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.CheckIn.Application.Contracts;

public interface ITranslateProvider
{
    Task<string> Get(MessageId messageId, LanguageId languageId, params object[] values);
}