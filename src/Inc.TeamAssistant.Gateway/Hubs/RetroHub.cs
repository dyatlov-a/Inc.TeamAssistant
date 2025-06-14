using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeTimer;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;
using Inc.TeamAssistant.Retro.Model.Commands.JoinToRetro;
using Inc.TeamAssistant.Retro.Model.Commands.LeaveFromAll;
using Inc.TeamAssistant.Retro.Model.Commands.LeaveFromRetro;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveRetroItem;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

[Authorize]
internal sealed class RetroHub : Hub<IRetroHubClient>
{
    private readonly IMediator _mediator;

    public RetroHub(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HubMethodName(HubDescriptors.RetroHub.JoinRetroMethod)]
    public async Task JoinRetro(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString("N"));

        await _mediator.Send(new JoinToRetroCommand(Context.ConnectionId, roomId), CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.LeaveRetroMethod)]
    public async Task LeaveRetro(Guid roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString("N"));
        
        await _mediator.Send(new LeaveFromRetroCommand(Context.ConnectionId, roomId), CancellationToken.None);
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
        await Clients.GroupExcept(roomId.ToString("N"), Context.ConnectionId).ItemMoved(itemId);
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
        await _mediator.Send(new RemoveActionItemCommand(itemId, roomId, Context.ConnectionId), CancellationToken.None);
    }

    [HubMethodName(HubDescriptors.RetroHub.ChangeTimerMethod)]
    public async Task ChangeTimer(ChangeTimerCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }
    
    [HubMethodName(HubDescriptors.RetroHub.GiveFacilitatorMethod)]
    public async Task GiveFacilitator(ChangeRetroPropertiesCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var result = await _mediator.Send(new LeaveFromAllCommand(Context.ConnectionId), CancellationToken.None);

        foreach (var roomId in result.RoomIds)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString("N"));

        await base.OnDisconnectedAsync(exception);
    }
}