namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBot;

public sealed record GetBotResult(
    Guid Id,
    string UserName,
    string Token,
    IReadOnlyCollection<Guid> FeatureIds,
    IReadOnlyDictionary<string, string> Properties,
    IReadOnlyCollection<string> SupportedLanguages);