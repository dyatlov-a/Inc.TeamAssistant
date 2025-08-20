using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IRenderContext
{
    bool IsClientSide { get; }
    
    LanguageId? SelectedLanguage { get; }
    
    LanguageId CurrentLanguage => SelectedLanguage ?? LanguageSettings.DefaultLanguageId;
}