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

	public async Task<EventsProvider> OnStoryChanged(Guid teamId, Func<Task> changed)
	{
        ArgumentNullException.ThrowIfNull(changed);

        _hubConnection.On("StoryChanged", changed);

		await _hubConnection.StartAsync();

		await _hubConnection.InvokeAsync("JoinToGroup", teamId);

		return this;
	}

	public ValueTask DisposeAsync() => _hubConnection.DisposeAsync();
}