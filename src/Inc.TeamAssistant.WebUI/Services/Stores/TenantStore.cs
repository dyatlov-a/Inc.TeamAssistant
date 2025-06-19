using Inc.TeamAssistant.Tenants.Model.Queries.Common;
using Inc.TeamAssistant.WebUI.Components;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.Internal;

namespace Inc.TeamAssistant.WebUI.Services.Stores;

internal sealed class TenantStore
{
    private IDictionary<Guid, RoomDto> _rooms = new Dictionary<Guid, RoomDto>();
    private readonly ITenantService _tenantService;
    private readonly RequestProcessor _requestProcessor;

    public TenantStore(ITenantService tenantService, RequestProcessor requestProcessor)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        _requestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
    }

    public IEnumerable<RoomDto> Rooms => _rooms.Values;
    public event Action? OnChange;

    public async Task Initialize(Guid? roomId, IProgress<LoadingState.State> progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        
        var result = await _requestProcessor.Process(
            async () => await _tenantService.GetAvailableRooms(roomId),
            nameof(TenantStore),
            progress);

        _rooms = result.Rooms.ToDictionary(t => t.Id);

        NotifyStateHasChanged();
    }
    
    private void NotifyStateHasChanged() => OnChange?.Invoke();
}