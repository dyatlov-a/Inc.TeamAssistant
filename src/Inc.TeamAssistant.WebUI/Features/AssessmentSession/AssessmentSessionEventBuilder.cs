using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.WebUI.Services.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Inc.TeamAssistant.WebUI.Features.AssessmentSession;

internal sealed class AssessmentSessionEventBuilder : IAssessmentSessionProvider, IAsyncDisposable
{
	private readonly HubConnection _hubConnection;

	public AssessmentSessionEventBuilder(NavigationManager navigationManager)
	{
        ArgumentNullException.ThrowIfNull(navigationManager);

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri(GlobalResources.Hubs.AssessmentSessionEvents))
            .WithAutomaticReconnect()
            .Build();
	}

	public Task Start() => _hubConnection.StartAsync();

	public async Task<IAsyncDisposable> Build(
		Guid teamId,
		params IReadOnlyCollection<Func<IAssessmentSessionProvider, IDisposable>> eventHandlers)
	{
        ArgumentNullException.ThrowIfNull(eventHandlers);

        var handlers = eventHandlers
	        .Select(i => i(this))
	        .ToArray();

		await _hubConnection.InvokeAsync("JoinToGroup", teamId);

		return new ActionScope(async () =>
		{
			await _hubConnection.InvokeAsync("RemoveFromGroup", teamId);
			
			foreach (var handler in handlers)
				handler.Dispose();
		});
	}

	IDisposable IAssessmentSessionProvider.OnStoryChanged(Func<Task> changed)
	{
		ArgumentNullException.ThrowIfNull(changed);
		
		return _hubConnection.On("StoryChanged", changed);
	}

	IDisposable IAssessmentSessionProvider.OnStoryAccepted(Func<string, Task> accepted)
	{
		ArgumentNullException.ThrowIfNull(accepted);
		
		return _hubConnection.On("StoryAccepted", accepted);
	}

	public ValueTask DisposeAsync() => _hubConnection.DisposeAsync();
}