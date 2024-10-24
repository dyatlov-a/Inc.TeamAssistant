namespace Inc.TeamAssistant.Primitives.FeatureProperties;

public sealed record SettingSection(
    string HeaderMessageId,
    string HelpMessageId,
    IReadOnlyCollection<SettingItem> SettingItems);