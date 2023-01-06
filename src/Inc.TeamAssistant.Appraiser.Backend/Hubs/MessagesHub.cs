using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Appraiser.Backend.Hubs;

public sealed class MessagesHub : Hub<IMessagesHubClient>
{
    public async Task JoinToGroup(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString("N"));
    }
}