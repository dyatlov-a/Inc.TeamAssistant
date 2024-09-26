namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Settings;

public sealed class DashboardSettingsItem
{
    public string Type { get; set; } = string.Empty;
    public string Feature { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool CanEnabled { get; set; }
    public bool IsVisible { get; set; }
}