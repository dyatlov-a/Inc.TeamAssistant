namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Settings;

public sealed class DashboardSettingsItem
{
    public string Type { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool CanEnabled { get; set; }
    public bool IsEnabled { get; set; }
}