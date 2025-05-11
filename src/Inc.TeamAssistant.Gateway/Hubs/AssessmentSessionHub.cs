using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class AssessmentSessionHub : Hub<IAssessmentSessionHubClient>
{
    [HubMethodName(HubDescriptors.AssessmentSessionHub.JoinToAssessmentSessionMethod)]
    public async Task JoinToAssessmentSession(Guid groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString("N"));
    }
    
    [HubMethodName(HubDescriptors.AssessmentSessionHub.RemoveFromAssessmentSessionMethod)]
    public async Task RemoveFromAssessmentSession(Guid groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId.ToString("N"));
    }
}