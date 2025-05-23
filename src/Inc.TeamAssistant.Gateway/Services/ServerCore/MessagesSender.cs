using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Gateway.Hubs;
using Inc.TeamAssistant.Primitives.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class MessagesSender : IMessagesSender
{
	private readonly IHubContext<MessagesHub, IMessagesHubClient> _hubContext;

	public MessagesSender(IHubContext<MessagesHub, IMessagesHubClient> hubContext)
	{
		_hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
	}

    public Task StoryChanged(Guid teamId)
    {
        _hubContext.Clients.Group(teamId.ToLinkSegment()).StoryChanged();

        return Task.CompletedTask;
    }

    public Task StoryAccepted(Guid teamId, string totalValue)
    {
	    ArgumentException.ThrowIfNullOrWhiteSpace(totalValue);
	    
	    _hubContext.Clients.Group(teamId.ToLinkSegment()).StoryAccepted(totalValue);

	    return Task.CompletedTask;
    }
}