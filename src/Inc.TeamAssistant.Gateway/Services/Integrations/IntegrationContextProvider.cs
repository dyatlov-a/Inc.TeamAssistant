using Inc.TeamAssistant.Primitives.Integrations;

namespace Inc.TeamAssistant.Gateway.Services.Integrations;

public sealed class IntegrationContextProvider
{
    private readonly IIntegrationsAccessor _integrationsAccessor;
    
    private IntegrationContext? _context;

    public IntegrationContextProvider(IIntegrationsAccessor integrationsAccessor)
    {
        _integrationsAccessor = integrationsAccessor ?? throw new ArgumentNullException(nameof(integrationsAccessor));
    }

    public async Task<bool> Ensure(string accessToken, string projectKey, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        
        _context = await _integrationsAccessor.Find(accessToken, projectKey, token);

        return _context is not null;
    }
    
    public IntegrationContext Get() => _context ?? throw new ApplicationException("Integration context is not set");
}