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

	public async Task<IAsyncDisposable> OnStoryChanged(Guid teamId, Func<Task> changed)
	{
        ArgumentNullException.ThrowIfNull(changed);

        var handler = _hubConnection.On("StoryChanged", changed);

		await _hubConnection.InvokeAsync("JoinToGroup", teamId);

		return new Scope(async () =>
		{
			await _hubConnection.InvokeAsync("RemoveFromGroup", teamId);
			handler.Dispose();
		});
	}

	public ValueTask DisposeAsync() => _hubConnection.DisposeAsync();
}