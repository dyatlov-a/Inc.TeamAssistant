using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModel
{
    public IReadOnlyCollection<SelectListItem> Properties { get; set; } = Array.Empty<SelectListItem>();
    
    public SetSettingsFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        Properties = stagesState.PropertyKeys
            .Select(v => new SelectListItem
            {
                Name = v,
                Value = stagesState.Properties.GetValueOrDefault(v, string.Empty)
            })
            .ToArray();

        return this;
    }
}