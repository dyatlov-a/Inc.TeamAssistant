using Inc.TeamAssistant.Primitives.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class MessagesHub : Hub<IMessagesHubClient>
{
    public async Task JoinToGroup(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToLinkSegment());
    }
    
    public async Task RemoveFromGroup(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToLinkSegment());
    }
}