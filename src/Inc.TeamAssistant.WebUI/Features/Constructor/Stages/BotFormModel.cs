namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class BotFormModel
{
    public Guid? Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public IReadOnlyCollection<Feature> Features { get; set; } = Array.Empty<Feature>();
    public IReadOnlyCollection<Guid> FeatureIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyCollection<string> PropertyKeys { get; set; } = Array.Empty<string>();
    public IReadOnlyDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    
    public sealed class Feature
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IReadOnlyCollection<string> Properties { get; set; } = Array.Empty<string>();
    }
}