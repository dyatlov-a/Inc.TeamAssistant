namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class Stage3SetSettingsFormModel
{
    public IReadOnlyCollection<Setting> Properties { get; set; } = default!;
    
    public sealed class Setting
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
}