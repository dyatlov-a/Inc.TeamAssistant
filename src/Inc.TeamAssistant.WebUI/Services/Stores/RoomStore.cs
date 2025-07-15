using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;
using Inc.TeamAssistant.WebUI.Components;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.Internal;

namespace Inc.TeamAssistant.WebUI.Services.Stores;

internal sealed class RoomStore
{
    private GetRoomPropertiesResult _roomProperties = GetRoomPropertiesResult.Empty;
    private readonly ITenantService _tenantService;
    private readonly RequestProcessor _requestProcessor;

    public RoomStore(ITenantService tenantService, RequestProcessor requestProcessor)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        _requestProcessor = requestProcessor ?? throw new ArgumentNullException(nameof(requestProcessor));
    }

    public GetRoomPropertiesResult RoomProperties => _roomProperties;
    public event Action? OnChange;

    public async Task Initialize(Guid roomId, IProgress<LoadingState.State> progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        
        _roomProperties = await _requestProcessor.Process(
            async () => await _tenantService.GetRoomProperties(roomId),
            nameof(TenantStore),
            progress);

        NotifyStateHasChanged();
    }
    
    private void NotifyStateHasChanged() => OnChange?.Invoke();
}