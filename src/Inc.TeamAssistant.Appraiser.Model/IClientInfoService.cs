using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model;

public interface IClientInfoService
{
    Task<LanguageId> GetCurrentLanguageId();

    LanguageId? GetLanguageIdFromUrlOrDefault();
}