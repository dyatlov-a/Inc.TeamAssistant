using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Gateway.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class AssessmentSessionEventSender : IAssessmentSessionEventSender
{
	private readonly IHubContext<AssessmentSessionHub, IAssessmentSessionHubClient> _hubContext;

	public AssessmentSessionEventSender(IHubContext<AssessmentSessionHub, IAssessmentSessionHubClient> hubContext)
	{
		_hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
	}

    public async Task StoryChanged(Guid teamId)
    {
        await _hubContext.Clients.Group(teamId.ToString("N")).StoryChanged();
    }

    public async Task StoryAccepted(Guid teamId, string totalValue)
    {
	    ArgumentException.ThrowIfNullOrWhiteSpace(totalValue);
	    
	    await _hubContext.Clients.Group(teamId.ToString("N")).StoryAccepted(totalValue);
    }
}