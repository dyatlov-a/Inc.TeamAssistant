namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class BotFormModel
{
    public Guid? Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<Guid> FeatureIds { get; set; } = new();
    public Dictionary<string, string> Properties { get; set; } = new();
}