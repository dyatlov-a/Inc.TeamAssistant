namespace Inc.TeamAssistant.Primitives.Features.Properties;

public sealed record SettingItem(
    string PropertyName,
    string LabelMessageId,
    IReadOnlyCollection<SelectValue> Values);