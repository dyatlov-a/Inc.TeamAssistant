using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IMessageBuilder
{
    Task<string> Build(MessageId messageId, LanguageId languageId, params object[] values);
}