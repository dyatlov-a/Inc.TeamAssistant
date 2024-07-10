namespace Inc.TeamAssistant.Primitives.Properties;

public interface ISettingSectionProvider
{
    string FeatureName { get; }
    
    IReadOnlyCollection<SettingSection> GetSections();
}