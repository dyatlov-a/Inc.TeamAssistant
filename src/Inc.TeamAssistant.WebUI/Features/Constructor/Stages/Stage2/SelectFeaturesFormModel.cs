namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage2;

public sealed class SelectFeaturesFormModel
{
    public List<Guid> FeatureIds { get; set; } = new();
    
    public SelectFeaturesFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);
        
        FeatureIds = stagesState.FeatureIds.ToList();

        return this;
    }
}