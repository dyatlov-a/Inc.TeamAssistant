namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage2;

public sealed class SelectFeaturesFormModel
{
    private readonly List<Guid> _featureIds = new();
    public IReadOnlyCollection<Guid> FeatureIds => _featureIds;
    
    public SelectFeaturesFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);
        
        _featureIds.Clear();
        _featureIds.AddRange(stagesState.FeatureIds);

        return this;
    }

    public void SetFeatures(IEnumerable<Guid> featureIds)
    {
        ArgumentNullException.ThrowIfNull(featureIds);
        
        _featureIds.Clear();
        _featureIds.AddRange(featureIds);
    }
}