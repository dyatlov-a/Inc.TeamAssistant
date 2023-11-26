using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Gateway.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Services;

internal sealed class MessagesSender : IMessagesSender
{
	private readonly IHubContext<MessagesHub, IMessagesHubClient> _hubContext;

	public MessagesSender(IHubContext<MessagesHub, IMessagesHubClient> hubContext)
		=> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public Task StoryChanged(Guid assessmentSessionId)
    {
        _hubContext.Clients.Group(assessmentSessionId.ToString("N")).StoryChanged();

        return Task.CompletedTask;
    }
}