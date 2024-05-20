namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModel
{
    public IReadOnlyCollection<Setting> Properties { get; set; } = Array.Empty<Setting>();
    
    public sealed class Setting
    {
        public string Name { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
    
    public SetSettingsFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        Properties = stagesState.PropertyKeys
            .Select(v => new Setting
            {
                Name = v,
                Value = stagesState.Properties.GetValueOrDefault(v)
            })
            .ToList();

        return this;
    }
}