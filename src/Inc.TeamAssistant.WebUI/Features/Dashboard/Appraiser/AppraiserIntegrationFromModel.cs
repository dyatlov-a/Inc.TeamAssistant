using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;
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

    public AppraiserIntegrationFromModel Apply(GetIntegrationPropertiesResult integration)
    {
        if (integration.Properties is not null)
        {
            AccessToken = integration.Properties.AccessToken;
            ProjectKey = integration.Properties.ProjectKey;
            ScrumMasterId = integration.Properties.ScrumMasterId;
        }
        else
        {
            AccessToken = string.Empty;
            ProjectKey = string.Empty;
            ScrumMasterId = 0;
        }
        
        IsEnabled = integration.Properties is not null;
        IsDisableControls = !integration.HasManagerAccess;
        Teammates = integration.Teammates;
        
        return this;
    }
}