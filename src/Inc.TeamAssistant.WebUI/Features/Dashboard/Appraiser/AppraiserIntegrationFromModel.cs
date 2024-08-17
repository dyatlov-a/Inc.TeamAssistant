using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public sealed class AppraiserIntegrationFromModel
{
    public bool IsDisableControls { get; set; }
    public bool IsEnabled { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string ProjectKey { get; set; } = string.Empty;
    public long ScrumMasterId { get; set; }
    public IReadOnlyCollection<Person> Teammates { get; set; } = Array.Empty<Person>();
}