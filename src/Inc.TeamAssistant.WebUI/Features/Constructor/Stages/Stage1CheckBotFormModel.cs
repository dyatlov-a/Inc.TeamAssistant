namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class Stage1CheckBotFormModel
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public bool HasAccess { get; set; }
}