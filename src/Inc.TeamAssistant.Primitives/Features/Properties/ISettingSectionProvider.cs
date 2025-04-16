namespace Inc.TeamAssistant.Primitives.Features.Properties;

public interface ISettingSectionProvider
{
    string FeatureName { get; }
    
    IReadOnlyCollection<SettingSection> GetSections();
}