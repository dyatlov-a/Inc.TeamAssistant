using Inc.TeamAssistant.Appraiser.Backend.Hubs;
using Inc.TeamAssistant.Appraiser.Notifications.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

internal sealed class MessagesSender : IMessagesSender
{
	private readonly IHubContext<MessagesHub, IMessagesHubClient> _hubContext;

	public MessagesSender(IHubContext<MessagesHub, IMessagesHubClient> hubContext)
		=> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public Task StoryChanged(AssessmentSessionId assessmentSessionId)
    {
        if (assessmentSessionId is null)
            throw new ArgumentNullException(nameof(assessmentSessionId));

        _hubContext.Clients.Group(assessmentSessionId.Value.ToString("N")).StoryChanged();

        return Task.CompletedTask;
    }
}