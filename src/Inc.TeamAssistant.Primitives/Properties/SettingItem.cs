namespace Inc.TeamAssistant.Primitives.Properties;

public sealed record SettingItem(
    string PropertyName,
    string LabelMessageId,
    IReadOnlyCollection<SelectListItem> Values);