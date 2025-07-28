using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.WebUI;
using Inc.TeamAssistant.WebUI.Contracts;
using Inc.TeamAssistant.WebUI.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Inc.TeamAssistant.Gateway.Hubs;

[Authorize]
internal sealed class SurveyHub : Hub<ISurveyHubClient>
{
    private readonly IMediator _mediator;
    private readonly IOnlinePersonStore _store;

    public SurveyHub(IMediator mediator, IOnlinePersonStore store)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _store = store ??  throw new ArgumentNullException(nameof(store));
    }
    
    [HubMethodName(HubDescriptors.SurveyHub.JoinSurveyMethod)]
    public async Task JoinSurvey(Guid roomId)
    {
        var retroRoomId = RoomId.CreateForSurvey(roomId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, retroRoomId.GroupName);
        
        _store.JoinToRoom(retroRoomId, Context.ConnectionId, Context.User!.ToPerson());
    }
    
    [HubMethodName(HubDescriptors.SurveyHub.LeaveSurveyMethod)]
    public async Task LeaveSurvey(Guid roomId)
    {
        var retroRoomId = RoomId.CreateForSurvey(roomId);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, retroRoomId.GroupName);
        
        _store.LeaveFromRoom(retroRoomId, Context.ConnectionId);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var roomIds = _store.LeaveFromRooms(Context.ConnectionId);

        foreach (var roomId in roomIds)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.GroupName);

        await base.OnDisconnectedAsync(exception);
    }
}