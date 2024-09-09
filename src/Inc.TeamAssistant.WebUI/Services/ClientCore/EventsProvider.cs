using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Inc.TeamAssistant.WebUI.Services.ClientCore;

internal sealed class EventsProvider : IOnGroupHandlerBuilder, IAsyncDisposable
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
		params Func<IOnGroupHandlerBuilder, IDisposable>[] onGroupHandlers)
	{
        ArgumentNullException.ThrowIfNull(onGroupHandlers);

        var handlers = onGroupHandlers
	        .Select(i => i(this))
	        .ToArray();

		await _hubConnection.InvokeAsync("JoinToGroup", teamId);

		return new Scope(async () =>
		{
			await _hubConnection.InvokeAsync("RemoveFromGroup", teamId);
			
			foreach (var handler in handlers)
				handler.Dispose();
		});
	}

	IDisposable IOnGroupHandlerBuilder.OnStoryChanged(Func<Task> changed)
	{
		ArgumentNullException.ThrowIfNull(changed);
		
		return _hubConnection.On("StoryChanged", changed);
	}

	IDisposable IOnGroupHandlerBuilder.OnStoryAccepted(Func<string, Task> accepted)
	{
		ArgumentNullException.ThrowIfNull(accepted);
		
		return _hubConnection.On("StoryAccepted", accepted);
	}

	public ValueTask DisposeAsync() => _hubConnection.DisposeAsync();
}