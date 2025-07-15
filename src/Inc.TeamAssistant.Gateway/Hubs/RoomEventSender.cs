using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

internal sealed class RoomEventSender : IRoomEventSender
{
    private readonly IHubContext<RetroHub, IRetroHubClient> _hubContext;
    
    public RoomEventSender(IHubContext<RetroHub, IRetroHubClient> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
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
    
    public async Task PropertiesChanged(Guid roomId, RoomPropertiesDto properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        await _hubContext.Clients.Group(roomId.ToString("N")).RetroPropertiesChanged(properties);
    }

    public async Task TimerChanged(Guid roomId, TimeSpan? duration)
    {
        await _hubContext.Clients.Group(roomId.ToString("N")).TimerChanged(duration);
    }
}