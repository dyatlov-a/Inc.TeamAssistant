using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Gateway.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Services;

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