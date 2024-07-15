namespace Inc.TeamAssistant.Primitives.FeatureProperties;

public interface ISettingSectionProvider
{
    string FeatureName { get; }
    
    IReadOnlyCollection<SettingSection> GetSections();
}