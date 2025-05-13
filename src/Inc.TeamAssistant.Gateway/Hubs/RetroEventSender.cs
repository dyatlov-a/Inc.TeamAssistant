using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class RetroEventSender : IRetroEventSender
{
    private readonly IHubContext<RetroHub, IRetroHubClient> _hubContext;

    public RetroEventSender(IHubContext<RetroHub, IRetroHubClient> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }
    
    public async Task RetroItemChanged(RetroItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        await _hubContext.Clients.Group(item.TeamId.ToString("N")).RetroItemChanged(item);
    }

    public async Task RetroItemRemoved(RetroItemDto item)
    {
        await _hubContext.Clients.Group(item.TeamId.ToString("N")).RetroItemRemoved(item);
    }

    public async Task RetroSessionChanged(RetroSessionDto session)
    {
        await _hubContext.Clients.Group(session.TeamId.ToString("N")).RetroSessionChanged(session);
    }
}