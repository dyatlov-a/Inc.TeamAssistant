using Inc.TeamAssistant.Tenants.Model.Queries.Common;
using Inc.TeamAssistant.WebUI.Components;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Services.Stores;

internal sealed class TenantStore
{
    private IDictionary<Guid, TenantTeamDto> _teams = new Dictionary<Guid, TenantTeamDto>();
    private readonly ITenantService _tenantService;
    private readonly RequestProcessor _requestProcessor;

    public TenantStore(ITenantService tenantService, RequestProcessor requestProcessor)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        _requestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
    }

    public IEnumerable<TenantTeamDto> Teams => _teams.Values;
    public event Action? OnChange;

    public async Task Initialize(Guid? teamId, IProgress<LoadingState.State> progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        
        var result = await _requestProcessor.Process(
            () => _tenantService.GetAvailableTeams(teamId),
            nameof(TenantStore),
            progress);

        _teams = result.Teams.ToDictionary(t => t.Id);

        NotifyStateHasChanged();
    }
    
    private void NotifyStateHasChanged() => OnChange?.Invoke();
}