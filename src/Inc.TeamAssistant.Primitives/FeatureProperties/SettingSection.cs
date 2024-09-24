using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives.FeatureProperties;

public sealed record SettingSection(
    MessageId HeaderMessageId,
    MessageId HelpMessageId,
    IReadOnlyCollection<SettingItem> SettingItems);