namespace Inc.TeamAssistant.Primitives.Features.Properties;

public sealed record SettingSection(
    string HeaderMessageId,
    string HelpMessageId,
    IReadOnlyCollection<SettingItem> SettingItems);