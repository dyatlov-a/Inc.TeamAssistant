using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

public sealed class MessagesHub : Hub<IMessagesHubClient>
{
    public async Task JoinToGroup(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString("N"));
    }
}