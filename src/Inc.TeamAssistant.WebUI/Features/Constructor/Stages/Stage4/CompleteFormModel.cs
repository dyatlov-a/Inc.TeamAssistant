using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CompleteFormModel
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public IReadOnlyCollection<Guid> FeatureIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public IReadOnlyCollection<string> SupportedLanguages { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<BotDetailsFormModel> BotDetails { get; set; } = Array.Empty<BotDetailsFormModel>();
    
    public CompleteFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        UserName = stagesState.UserName;
        Token = stagesState.Token;
        FeatureIds = stagesState.FeatureIds.ToArray();
        Properties = stagesState.SelectedFeatures
            .SelectMany(f => f.Properties.Select(p => (
                Key: p,
                Value: stagesState.Properties.GetValueOrDefault(p, string.Empty))))
            .ToDictionary(f => f.Key, f => f.Value, StringComparer.InvariantCultureIgnoreCase);
        SupportedLanguages = stagesState.SupportedLanguages.ToArray();
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