namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class Stage4CompleteFormModel
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public IReadOnlyCollection<Guid> FeatureIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
}