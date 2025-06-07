using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.GiveFacilitator;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;
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
    private readonly ILogger<RetroEventBuilder> _logger;
    private Action? _rerender;
    private Func<Task>? _reload;

    public RetroEventBuilder(NavigationManager navigationManager, ILogger<RetroEventBuilder> logger)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri(HubDescriptors.RetroHub.Endpoint))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.Closed += Closed;
        _hubConnection.Reconnected += Reconnected;
    }
    
    public bool IsDisconnected { get; private set; }

    public async Task<RetroEventBuilder> Start(Action rerender, Func<Task> reload)
    {
        _rerender = rerender ?? throw new ArgumentNullException(nameof(rerender));
        _reload = reload ?? throw new ArgumentNullException(nameof(reload));
        
        var hasDisconnect = IsDisconnected;
        
        try
        {
            await _hubConnection.StartAsync();

            IsDisconnected = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while starting RetroEventBuilder");
            
            IsDisconnected = true;
        }

        if (hasDisconnect)
            await _reload();

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
    
    public async Task SetRetroState(SetRetroStateCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.SetRetroStateMethod, command);
    }
    
    public async Task MoveItem(Guid teamId, Guid itemId)
    {
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.MoveItemMethod, teamId, itemId);
    }
    
    public async Task ChangeActionItem(ChangeActionItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.ChangeActionItemMethod, command);
    }

    public async Task RemoveActionItem(Guid teamId, Guid itemId)
    {
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.RemoveActionItemMethod, teamId, itemId);
    }
    
    public async Task ChangeTimer(Guid teamId, TimeSpan? duration)
    {
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.ChangeTimerMethod, teamId, duration);
    }
    
    public async Task GiveFacilitator(GiveFacilitatorCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _hubConnection.SendAsync(HubDescriptors.RetroHub.GiveFacilitatorMethod, command);
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

    public IDisposable OnRetroStateChanged(Func<long, bool, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
		
        return _hubConnection.On(nameof(IRetroHubClient.RetroStateChanged), changed);
    }

    IDisposable IRetroEventProvider.OnPersonsChanged(Func<IReadOnlyCollection<Person>, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
        
        return _hubConnection.On(nameof(IRetroHubClient.PersonsChanged), changed);
    }

    public IDisposable OnItemMoved(Func<Guid, Task> moved)
    {
        ArgumentNullException.ThrowIfNull(moved);
        
        return _hubConnection.On(nameof(IRetroHubClient.ItemMoved), moved);
    }

    public IDisposable OnActionItemChanged(Func<ActionItemDto, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
        
        return _hubConnection.On(nameof(IRetroHubClient.ActionItemChanged), changed);
    }
    
    public IDisposable OnActionItemRemoved(Func<Guid, Task> removed)
    {
        ArgumentNullException.ThrowIfNull(removed);
        
        return _hubConnection.On(nameof(IRetroHubClient.ActionItemRemoved), removed);
    }

    public IDisposable OnTimerChanged(Func<TimeSpan?, Task> changed)
    {
        ArgumentNullException.ThrowIfNull(changed);
        
        return _hubConnection.On(nameof(IRetroHubClient.TimerChanged), changed);
    }

    public IDisposable OnFacilitatorChanged(Func<long, Task> changed)
    {
        return _hubConnection.On(nameof(IRetroHubClient.FacilitatorChanged), changed);
    }

    private Task Closed(Exception? ex)
    {
        IsDisconnected = true;

        _rerender?.Invoke();
        
        return Task.CompletedTask;
    }
    
    private async Task Reconnected(string? connectionId)
    {
        IsDisconnected = false;

        if (_reload is not null)
            await _reload();
    }

    public ValueTask DisposeAsync()
    {
        _hubConnection.Closed -= Closed;
        _hubConnection.Reconnected -= Reconnected;
        
        return _hubConnection.DisposeAsync();
    }
}