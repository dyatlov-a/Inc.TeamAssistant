using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
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

    private readonly List<Person> _teammates = new();
    public IReadOnlyCollection<Person> Teammates => _teammates;

    public AppraiserIntegrationFromModel Apply(GetIntegrationPropertiesResult integration)
    {
        ArgumentNullException.ThrowIfNull(integration);
        
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
        
        _teammates.Clear();
        _teammates.AddRange(integration.Teammates);
        
        return this;
    }

    public AppraiserIntegrationFromModel Clear()
    {
        IsEnabled = false;
        AccessToken = string.Empty;
        ProjectKey = string.Empty;
        ScrumMasterId = 0;

        return this;
    } 

    public SetIntegrationPropertiesCommand ToCommand(Guid teamId)
    {
        return new SetIntegrationPropertiesCommand(
            teamId,
            ProjectKey,
            ScrumMasterId);
    }
}