using Inc.TeamAssistant.Primitives.FeatureProperties;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed record SetSettingsViewModel(
    IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> SettingSections,
    string MoveNextTitle,
    string SupportedLanguages,
    string SupportedLanguagesHelp,
    string Languages,
    string BotName,
    string BotShortDescription,
    string BotDescription)
    : IViewModel<SetSettingsViewModel>
{
    public static SetSettingsViewModel Empty { get; } = new(
        new Dictionary<string, IReadOnlyCollection<SettingSection>>(),
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}