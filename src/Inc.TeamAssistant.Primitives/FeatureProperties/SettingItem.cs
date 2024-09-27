using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives.FeatureProperties;

public sealed record SettingItem(
    string PropertyName,
    MessageId LabelMessageId,
    IReadOnlyCollection<SelectValue> Values);