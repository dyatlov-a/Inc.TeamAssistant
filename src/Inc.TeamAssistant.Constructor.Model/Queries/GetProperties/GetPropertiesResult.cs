using Inc.TeamAssistant.Primitives.FeatureProperties;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetProperties;

public sealed record GetPropertiesResult(IReadOnlyDictionary<string, IReadOnlyCollection<SettingSection>> Properties);