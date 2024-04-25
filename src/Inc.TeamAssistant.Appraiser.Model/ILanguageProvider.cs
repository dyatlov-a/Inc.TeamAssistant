using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Model;

public interface ILanguageProvider
{
    (LanguageId Language, bool Selected) GetCurrentLanguageId();
}