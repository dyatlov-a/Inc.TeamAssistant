using Microsoft.AspNetCore.SignalR.Client;

namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal static class HubDisconnectedStates
{
    public static HubConnectionState Connecting = HubConnectionState.Connecting;
    public static HubConnectionState Reconnecting = HubConnectionState.Reconnecting;
    public static HubConnectionState Disconnected = HubConnectionState.Disconnected;
    
    public static readonly IReadOnlyCollection<HubConnectionState> All = [Connecting, Reconnecting, Disconnected];
}