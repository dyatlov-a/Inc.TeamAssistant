using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class RetroEventSender : IRetroEventSender
{
    private readonly IHubContext<RetroHub, IRetroHubClient> _hubContext;
    private readonly IOnlinePersonStore _onlinePersonStore;

    public RetroEventSender(IHubContext<RetroHub, IRetroHubClient> hubContext, IOnlinePersonStore onlinePersonStore)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
    }
    
    public async Task RetroItemChanged(RetroItemDto item, bool excludedOwner = false)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var ownerConnectionId = _onlinePersonStore.FindConnectionId(item.RoomId, item.OwnerId);

        if (excludedOwner && !string.IsNullOrWhiteSpace(ownerConnectionId))
            await _hubContext.Clients.GroupExcept(item.RoomId.ToString("N"), ownerConnectionId).RetroItemChanged(item);
        else
            await _hubContext.Clients.Group(item.RoomId.ToString("N")).RetroItemChanged(item);
    }

    public async Task RetroItemRemoved(Guid roomId, Guid itemId)
    {
        await _hubContext.Clients.Group(roomId.ToString("N")).RetroItemRemoved(itemId);
    }

    public async Task RetroSessionChanged(RetroSessionDto session)
    {
        await _hubContext.Clients.Group(session.RoomId.ToString("N")).RetroSessionChanged(session);
    }

    public async Task VotesChanged(Guid roomId, long personId, int votesCount)
    {
        await _hubContext.Clients.Group(roomId.ToString("N")).VotesChanged(personId, votesCount);
    }

    public async Task RetroStateChanged(Guid roomId, long personId, bool finished, bool handRaised)
    {
        await _hubContext.Clients.Group(roomId.ToString("N")).RetroStateChanged(personId, finished, handRaised);
    }

    public async Task PersonsChanged(Guid roomId, IReadOnlyCollection<Person> persons)
    {
        ArgumentNullException.ThrowIfNull(persons);
        
        await _hubContext.Clients.Group(roomId.ToString("N")).PersonsChanged(persons);
    }

    public async Task ActionItemChanged(Guid roomId, ActionItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        await _hubContext.Clients.Group(roomId.ToString("N")).ActionItemChanged(item);
    }

    public async Task ActionItemRemoved(Guid roomId, Guid itemId, string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);
        
        await _hubContext.Clients.GroupExcept(roomId.ToString("N"), connectionId).ActionItemRemoved(itemId);
    }

    public async Task RoomPropertiesChanged(Guid roomId)
    {
        await _hubContext.Clients.Group(roomId.ToString("N")).RoomPropertiesChanged();
    }

    public async Task TimerChanged(Guid roomId, TimeSpan? duration)
    {
        await _hubContext.Clients.Group(roomId.ToString("N")).TimerChanged(duration);
    }
}