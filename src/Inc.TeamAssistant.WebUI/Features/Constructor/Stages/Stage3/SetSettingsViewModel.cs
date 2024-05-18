using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed record SetSettingsViewModel(
    IReadOnlyDictionary<string, SetSettingsViewModel.SettingSection> Items,
    string MoveNextTitle)
    : IViewModel<SetSettingsViewModel>
{
    public static SetSettingsViewModel Empty { get; } = new(new Dictionary<string, SettingSection>(), string.Empty);

    public sealed record SettingSection(string Header, string Help, IReadOnlyCollection<SettingItem> SettingItems);

    public sealed record SettingItem(string PropertyName, string Label, IReadOnlyCollection<SelectListItem> Values);

    public sealed record SelectListItem(string Text, string Value);
}