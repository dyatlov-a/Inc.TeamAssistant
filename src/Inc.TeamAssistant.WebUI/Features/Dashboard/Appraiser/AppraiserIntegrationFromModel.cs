using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public sealed class AppraiserIntegrationFromModel
{
    public bool HasManagerAccess { get; private set; }
    public bool IsEnabled { get; private set; }
    public string AccessToken { get; set; } = string.Empty;
    public string ProjectKey { get; set; } = string.Empty;
    public long ScrumMasterId { get; set; }

    private readonly List<Person> _teammates = new();
    public IReadOnlyCollection<Person> Teammates => _teammates;

    public AppraiserIntegrationFromModel Apply(GetIntegrationPropertiesResult integration)
    {
        ArgumentNullException.ThrowIfNull(integration);
        
        HasManagerAccess = integration.HasManagerAccess;
        
        _teammates.Clear();
        _teammates.AddRange(integration.Teammates);
        
        return integration.Properties is null
            ? Clear()
            : Apply(integration.Properties);
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

    public AppraiserIntegrationFromModel Enable()
    {
        IsEnabled = true;
        
        return this;
    }

    private AppraiserIntegrationFromModel Apply(IntegrationProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        AccessToken = properties.AccessToken;
        ProjectKey = properties.ProjectKey;
        ScrumMasterId = properties.ScrumMasterId;
        IsEnabled = true;

        return this;
    }
}