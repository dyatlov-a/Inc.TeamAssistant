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
        
        var ownerConnectionId = _onlinePersonStore.FindConnectionId(item.TeamId, item.OwnerId);

        if (excludedOwner && !string.IsNullOrWhiteSpace(ownerConnectionId))
            await _hubContext.Clients.GroupExcept(item.TeamId.ToString("N"), ownerConnectionId).RetroItemChanged(item);
        else
            await _hubContext.Clients.Group(item.TeamId.ToString("N")).RetroItemChanged(item);
    }

    public async Task RetroItemRemoved(Guid teamId, Guid columnId, Guid itemId)
    {
        await _hubContext.Clients.Group(teamId.ToString("N")).RetroItemRemoved(columnId, itemId);
    }

    public async Task RetroSessionChanged(RetroSessionDto session)
    {
        await _hubContext.Clients.Group(session.TeamId.ToString("N")).RetroSessionChanged(session);
    }

    public async Task VotesChanged(Guid teamId, long personId, int votesCount)
    {
        await _hubContext.Clients.Group(teamId.ToString("N")).VotesChanged(personId, votesCount);
    }

    public async Task PersonsChanged(Guid teamId, IReadOnlyCollection<Person> persons)
    {
        ArgumentNullException.ThrowIfNull(persons);
        
        await _hubContext.Clients.Group(teamId.ToString("N")).PersonsChanged(persons);
    }
}