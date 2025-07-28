using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeTimer;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

[Authorize]
internal sealed class RetroHub : Hub<IRetroHubClient>
{
    private readonly IMediator _mediator;
    private readonly IOnlinePersonStore _store;

    public RetroHub(IMediator mediator, IOnlinePersonStore store)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    [HubMethodName(HubDescriptors.RetroHub.JoinRetroMethod)]
    public async Task JoinRetro(Guid roomId)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, retroRoomId.GroupName);
        
        var persons = _store.JoinToRoom(retroRoomId, Context.ConnectionId, Context.User!.ToPerson());
        
        await Clients.Group(retroRoomId.GroupName).PersonsChanged(persons);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.LeaveRetroMethod)]
    public async Task LeaveRetro(Guid roomId)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, retroRoomId.GroupName);
        
        var persons = _store.LeaveFromRoom(retroRoomId, Context.ConnectionId);
        
        await Clients.Group(retroRoomId.GroupName).PersonsChanged(persons);
    }

    [HubMethodName(HubDescriptors.RetroHub.CreateRetroItemMethod)]
    public async Task CreateRetroItem(CreateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }

    [HubMethodName(HubDescriptors.RetroHub.UpdateRetroItemMethod)]
    public async Task UpdateRetroItem(UpdateRetroItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.RemoveRetroItemMethod)]
    public async Task RemoveRetroItem(Guid itemId)
    {
        await _mediator.Send(new RemoveRetroItemCommand(itemId), CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.SetVotesMethod)]
    public async Task SetVotes(SetVotesCommand command)
    {
        await _mediator.Send(command, CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.SetRetroStateMethod)]
    public async Task SetRetroState(SetRetroStateCommand command)
    {
        await _mediator.Send(command, CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.MoveItemMethod)]
    public async Task MoveItem(Guid roomId, Guid itemId)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await Clients.GroupExcept(retroRoomId.GroupName, Context.ConnectionId).ItemMoved(itemId);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.ChangeActionItemMethod)]
    public async Task ChangeActionItem(ChangeActionItemCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.RemoveActionItemMethod)]
    public async Task RemoveActionItem(Guid roomId, Guid itemId)
    {
        await _mediator.Send(new RemoveActionItemCommand(itemId, roomId), CancellationToken.None);
    }

    [HubMethodName(HubDescriptors.RetroHub.ChangeTimerMethod)]
    public async Task ChangeTimer(ChangeTimerCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.GiveFacilitatorMethod)]
    public async Task GiveFacilitator(ChangeRoomPropertiesCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }

    [HubMethodName(HubDescriptors.RetroHub.NotifyRetroPropertiesChanged)]
    public async Task NotifyRetroPropertiesChanged(Guid roomId)
    {
        var retroRoomId = RoomId.CreateForRetro(roomId);
        
        await Clients.Group(retroRoomId.GroupName).RetroPropertiesChanged();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var roomIds = _store.LeaveFromRooms(Context.ConnectionId);

        foreach (var roomId in roomIds)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.GroupName);
            
            var persons = _store.GetPersons(roomId);
        
            await Clients.Group(roomId.GroupName).PersonsChanged(persons);
        }

        await base.OnDisconnectedAsync(exception);
    }
}