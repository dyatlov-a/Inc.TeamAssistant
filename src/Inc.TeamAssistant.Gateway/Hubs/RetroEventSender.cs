using Inc.TeamAssistant.Primitives.Features.Tenants;
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
    
    public async Task RetroItemChanged(RetroItemDto item, EventTarget eventTarget)
    {
        ArgumentNullException.ThrowIfNull(item);

        var retroRoomId = RoomId.CreateForRetro(item.RoomId);
        var ownerConnectionId = _onlinePersonStore.FindConnectionId(retroRoomId, item.OwnerId);
        var client = eventTarget switch
        {
            EventTarget.Owner when !string.IsNullOrWhiteSpace(ownerConnectionId)
                => _hubContext.Clients.Client(ownerConnectionId),
            EventTarget.Participants when !string.IsNullOrWhiteSpace(ownerConnectionId)
                => _hubContext.Clients.GroupExcept(retroRoomId.GroupName, ownerConnectionId),
            _ => _hubContext.Clients.Group(retroRoomId.GroupName)
        };

        await client.RetroItemChanged(item);
    }

    public async Task RetroItemRemoved(Guid roomId, Guid itemId)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await _hubContext.Clients.Group(retroRoomId.GroupName).RetroItemRemoved(itemId);
    }

    public async Task RetroSessionChanged(RetroSessionDto session)
    {
        var retroRoomId = RoomId.CreateForRetro(session.RoomId);
        
        await _hubContext.Clients.Group(retroRoomId.GroupName).RetroSessionChanged(session);
    }

    public async Task VotesChanged(Guid roomId, long personId, int votesCount)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await _hubContext.Clients.Group(retroRoomId.GroupName).VotesChanged(personId, votesCount);
    }

    public async Task RetroStateChanged(Guid roomId, long personId, bool finished, bool handRaised)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await _hubContext.Clients.Group(retroRoomId.GroupName).RetroStateChanged(personId, finished, handRaised);
    }

    public async Task ActionItemChanged(Guid roomId, ActionItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await _hubContext.Clients.Group(retroRoomId.GroupName).ActionItemChanged(item);
    }

    public async Task ActionItemRemoved(Guid roomId, Guid itemId)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await _hubContext.Clients.Group(retroRoomId.GroupName).ActionItemRemoved(itemId);
    }

    public async Task TimerChanged(Guid roomId, TimeSpan? duration)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await _hubContext.Clients.Group(retroRoomId.GroupName).TimerChanged(duration);
    }
}