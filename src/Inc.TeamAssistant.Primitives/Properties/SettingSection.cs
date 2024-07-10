namespace Inc.TeamAssistant.Primitives.Properties;

public sealed record SettingSection(
    string HeaderMessageId,
    string HelpMessageId,
    IReadOnlyCollection<SettingItem> SettingItems);