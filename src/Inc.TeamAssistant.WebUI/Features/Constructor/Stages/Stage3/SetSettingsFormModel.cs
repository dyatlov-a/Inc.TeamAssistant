using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class SetSettingsFormModel
{
    public IReadOnlyCollection<SelectListItem> Properties { get; set; } = Array.Empty<SelectListItem>();
    public List<string> SupportedLanguages { get; set; } = new();
    public IReadOnlyCollection<BotDetailsFormModel> BotDetails { get; set; } = Array.Empty<BotDetailsFormModel>();

    public SetSettingsFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        Properties = stagesState.SelectedFeatures
            .SelectMany(f => f.Properties.Select(v => new SelectListItem
            {
                Name = v,
                Value = stagesState.Properties.GetValueOrDefault(v, string.Empty)
            }))
            .ToArray();
        SupportedLanguages = stagesState.SupportedLanguages.ToList();
        BotDetails = stagesState.BotDetails.Select(b => new BotDetailsFormModel
        {
            LanguageId = b.LanguageId,
            Name = b.Name,
            ShortDescription = b.ShortDescription,
            Description = b.Description
        }).ToArray();

        return this;
    }
}