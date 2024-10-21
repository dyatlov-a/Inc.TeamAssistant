namespace Inc.TeamAssistant.Primitives.FeatureProperties;

public sealed record SettingItem(
    string PropertyName,
    string LabelMessageId,
    IReadOnlyCollection<SelectValue> Values);