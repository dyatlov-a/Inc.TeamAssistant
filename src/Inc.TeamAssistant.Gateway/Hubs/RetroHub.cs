using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class RetroHub : Hub<IRetroHubClient>
{
    [HubMethodName(HubDescriptors.RetroHub.JoinToRetroMethod)]
    public async Task JoinToRetro(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString("N"));
    }
    
    [HubMethodName(HubDescriptors.RetroHub.RemoveFromRetroMethod)]
    public async Task RemoveFromRetro(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString("N"));
    }
}