namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class CompleteFormModel
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public IReadOnlyCollection<Guid> FeatureIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public IReadOnlyCollection<string> SupportedLanguages { get; set; } = Array.Empty<string>();
    
    public CompleteFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        UserName = stagesState.UserName;
        Token = stagesState.Token;
        FeatureIds = stagesState.FeatureIds.ToArray();
        Properties = stagesState.Properties.ToDictionary();
        SupportedLanguages = stagesState.SupportedLanguages.ToArray();

        return this;
    }
}