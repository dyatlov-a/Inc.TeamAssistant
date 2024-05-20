using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed record SetSettingsViewModel(
    IReadOnlyDictionary<string, BotSettingsFactory.SettingSection> SettingSections,
    string MoveNextTitle)
    : IViewModel<SetSettingsViewModel>
{
    public static SetSettingsViewModel Empty { get; } = new(
        new Dictionary<string, BotSettingsFactory.SettingSection>(),
        string.Empty);
}