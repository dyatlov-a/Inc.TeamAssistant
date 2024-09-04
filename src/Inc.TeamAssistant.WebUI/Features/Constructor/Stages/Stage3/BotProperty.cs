namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed record BotProperty
{
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
}