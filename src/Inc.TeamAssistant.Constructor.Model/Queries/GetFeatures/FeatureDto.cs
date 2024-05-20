namespace Inc.TeamAssistant.Constructor.Model.Queries.GetFeatures;

public sealed record FeatureDto(Guid Id, string Name, IReadOnlyCollection<string> Properties);