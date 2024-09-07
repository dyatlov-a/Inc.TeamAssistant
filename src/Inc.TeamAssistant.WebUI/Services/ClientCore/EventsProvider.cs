using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

internal sealed class EventsProvider : IAsyncDisposable
{
	private readonly HubConnection _hubConnection;

	public EventsProvider(NavigationManager navigationManager)
	{
        ArgumentNullException.ThrowIfNull(navigationManager);

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/messages"))
            .WithAutomaticReconnect()
            .Build();
	}

	public Task Start() => _hubConnection.StartAsync();

	public async Task<IAsyncDisposable> OnStoryChanged(
		Guid teamId,
		Func<Task> changed,
		Func<string, Task> accepted)
	{
        ArgumentNullException.ThrowIfNull(changed);
        ArgumentNullException.ThrowIfNull(accepted);

        var storyChangedHandler = _hubConnection.On("StoryChanged", changed);
        var storyAcceptedHandler = _hubConnection.On("StoryAccepted", accepted);

		await _hubConnection.InvokeAsync("JoinToGroup", teamId);

		return new Scope(async () =>
		{
			await _hubConnection.InvokeAsync("RemoveFromGroup", teamId);
			storyChangedHandler.Dispose();
			storyAcceptedHandler.Dispose();
		});
	}

	public ValueTask DisposeAsync() => _hubConnection.DisposeAsync();
}