using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRenderContext
{
    (LanguageId Language, bool Selected) GetCurrentLanguageId();
    
    bool IsBrowser { get; }
}