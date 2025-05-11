using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

internal sealed class AssessmentSessionEventBuilder : IAssessmentSessionEventProvider, IAsyncDisposable
{
	private readonly HubConnection _hubConnection;

	public AssessmentSessionEventBuilder(NavigationManager navigationManager)
	{
        ArgumentNullException.ThrowIfNull(navigationManager);

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri(HubDescriptors.AssessmentSessionHub.Endpoint))
            .WithAutomaticReconnect()
            .Build();
	}

	public Task Start() => _hubConnection.StartAsync();

	public async Task<IAsyncDisposable> Build(
		Guid teamId,
		params IReadOnlyCollection<Func<IAssessmentSessionEventProvider, IDisposable>> eventHandlers)
	{
        ArgumentNullException.ThrowIfNull(eventHandlers);

        var handlers = eventHandlers
	        .Select(i => i(this))
	        .ToArray();

		await _hubConnection.InvokeAsync(HubDescriptors.AssessmentSessionHub.JoinToAssessmentSessionMethod, teamId);

		return new PostActionScope(async () =>
		{
			await _hubConnection.InvokeAsync(HubDescriptors.AssessmentSessionHub.RemoveFromAssessmentSessionMethod, teamId);
			
			foreach (var handler in handlers)
				handler.Dispose();
		});
	}

	IDisposable IAssessmentSessionEventProvider.OnStoryChanged(Func<Task> changed)
	{
		ArgumentNullException.ThrowIfNull(changed);
		
		return _hubConnection.On(nameof(IAssessmentSessionHubClient.StoryChanged), changed);
	}

	IDisposable IAssessmentSessionEventProvider.OnStoryAccepted(Func<string, Task> accepted)
	{
		ArgumentNullException.ThrowIfNull(accepted);
		
		return _hubConnection.On(nameof(IAssessmentSessionHubClient.StoryAccepted), accepted);
	}

	public ValueTask DisposeAsync() => _hubConnection.DisposeAsync();
}