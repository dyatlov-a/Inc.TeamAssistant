using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.Tenants.Application.Contracts;
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
        
        var ownerConnectionId = _onlinePersonStore.FindConnectionId(item.RoomId, item.OwnerId);
        var client = eventTarget switch
        {
            EventTarget.Owner when !string.IsNullOrWhiteSpace(ownerConnectionId)
                => _hubContext.Clients.Client(ownerConnectionId),
            EventTarget.Participants when !string.IsNullOrWhiteSpace(ownerConnectionId)
                => _hubContext.Clients.GroupExcept(item.RoomId.ToString("N"), ownerConnectionId),
            _ => _hubContext.Clients.Group(item.RoomId.ToString("N"))
        };

        await client.RetroItemChanged(item);
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

    public async Task ActionItemChanged(Guid roomId, ActionItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        await _hubContext.Clients.Group(roomId.ToString("N")).ActionItemChanged(item);
    }

    public async Task ActionItemRemoved(Guid roomId, Guid itemId)
    {
        await _hubContext.Clients.Group(roomId.ToString("N")).ActionItemRemoved(itemId);
    }
}