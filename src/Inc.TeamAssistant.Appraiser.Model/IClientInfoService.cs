using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Model;

public interface IClientInfoService
{
    Task<LanguageId> GetCurrentLanguageId();

    LanguageId? GetLanguageIdFromUrlOrDefault();
}