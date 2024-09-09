using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRenderContext
{
    (LanguageId CurrentLanguage, bool Selected) GetLanguageContext();
    
    bool IsBrowser { get; }

    bool IsDevelopment();
}