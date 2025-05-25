using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Services.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

internal sealed class RetroEventBuilder : IRetroEventProvider, IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private Action? _changed;

    public RetroEventBuilder(NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri(HubDescriptors.RetroHub.Endpoint))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.Closed += Closed;
        _hubConnection.Reconnected += Reconnected;
    }
    
    public bool IsDisconnected { get; private set; }

    public async Task<RetroEventBuilder> Start(Action changed)
    {
        _changed = changed ?? throw new ArgumentNullException(nameof(changed));
        
        try
        {
            await _hubConnection.StartAsync();

            IsDisconnected = false;
        }
        catch
        {
            IsDisconnected = true;
        }

        _changed();

        return this;
    }

    public async Task CreateRetroItem(CreateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.CreateRetroItemMethod, command);
    }
    
    public async Task UpdateRetroItem(UpdateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.UpdateRetroItemMethod, command);
    }
    
    public async Task RemoveRetroItem(Guid itemId)
    {
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.RemoveRetroItemMethod, itemId);
    }

    public async Task SetVotes(SetVotesCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.SetVotesMethod, command);
    }

    public async Task<IAsyncDisposable> Build(
        Guid teamId,
        params IReadOnlyCollection<Func<IRetroEventProvider, IDisposable>> eventHandlers)
    {
        ArgumentNullException.ThrowIfNull(eventHandlers);

        var handlers = eventHandlers
            .Select(i => i(this))
            .ToArray();

        await _hubConnection.InvokeAsync(HubDescriptors.RetroHub.JoinRetroMethod, teamId);

        return new PostActionScope(async () =>
        {
            await _hubConnection.InvokeAsync(HubDescriptors.RetroHub.LeaveRetroMethod, teamId);
			
            foreach (var handler in handlers)
                handler.Dispose();
        });
    }
    
    IDisposable IRetroEventProvider.OnRetroItemChanged(Func<RetroItemDto, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
		
        return _hubConnection.On(nameof(IRetroHubClient.RetroItemChanged), changed);
    }

    IDisposable IRetroEventProvider.OnRetroItemRemoved(Func<Guid, Task> removed)
    {
        ArgumentNullException.ThrowIfNull(removed);
		
        return _hubConnection.On(nameof(IRetroHubClient.RetroItemRemoved), removed);
    }

    IDisposable IRetroEventProvider.OnRetroSessionChanged(Func<RetroSessionDto, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
		
        return _hubConnection.On(nameof(IRetroHubClient.RetroSessionChanged), changed);
    }

    IDisposable IRetroEventProvider.OnVotesChanged(Func<long, int, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
		
        return _hubConnection.On(nameof(IRetroHubClient.VotesChanged), changed);
    }

    IDisposable IRetroEventProvider.OnPersonsChanged(Func<IReadOnlyCollection<Person>, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
        
        return _hubConnection.On(nameof(IRetroHubClient.PersonsChanged), changed);
    }
    
    private Task Closed(Exception? ex)
    {
        IsDisconnected = true;

        _changed?.Invoke();
        
        return Task.CompletedTask;
    }
    
    private Task Reconnected(string? connectionId)
    {
        IsDisconnected = false;
        
        _changed?.Invoke();
        
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _hubConnection.Closed -= Closed;
        _hubConnection.Reconnected -= Reconnected;
        
        return _hubConnection.DisposeAsync();
    }
}