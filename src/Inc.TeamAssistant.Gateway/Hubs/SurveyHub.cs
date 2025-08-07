using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
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
    private readonly IPersonState _personState;

    public SurveyHub(IMediator mediator, IOnlinePersonStore store, IPersonState personState)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _store = store ??  throw new ArgumentNullException(nameof(store));
        _personState = personState ??  throw new ArgumentNullException(nameof(personState));
    }
    
    [HubMethodName(HubDescriptors.SurveyHub.JoinSurveyMethod)]
    public async Task JoinSurvey(Guid roomId)
    {
        var surveyRoomId = RoomId.CreateForSurvey(roomId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, surveyRoomId.GroupName);
        
        _store.JoinToRoom(surveyRoomId, Context.ConnectionId, Context.User!.ToPerson());
        
        await Clients.Group(surveyRoomId.GroupName).PersonsChanged(GetTickets(surveyRoomId));
    }
    
    [HubMethodName(HubDescriptors.SurveyHub.GiveFacilitatorMethod)]
    public async Task GiveFacilitator(Guid roomId)
    {
        var surveyRoomId = RoomId.CreateForSurvey(roomId);
        var person = Context.User!.ToPerson();
        
        await _mediator.Send(ChangeRoomPropertiesCommand.ChangeFacilitator(roomId), CancellationToken.None);

        await Clients.Group(surveyRoomId.GroupName).FacilitatorChanged(person.Id);
    }
    
    [HubMethodName(HubDescriptors.SurveyHub.SetAnswerMethod)]
    public async Task SetAnswer(SetAnswerCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        await _mediator.Send(command, CancellationToken.None);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var roomIds = _store.LeaveFromRooms(Context.ConnectionId);

        foreach (var roomId in roomIds)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.GroupName);
            
            await Clients.Group(roomId.GroupName).PersonsChanged(GetTickets(roomId));
        }

        await base.OnDisconnectedAsync(exception);
    }

    private IReadOnlyCollection<PersonStateTicket> GetTickets(RoomId roomId)
    {
        ArgumentNullException.ThrowIfNull(roomId);
        
        var onlinePersons = _store.GetPersons(roomId);
        var personState = _personState.Get(roomId).ToDictionary(p => p.Person.Id);
        
        var persons = onlinePersons
            .Select(op => personState.TryGetValue(op.Id, out var state)
                ? state
                : new PersonStateTicket(op, Finished: false, HandRaised: false))
            .ToArray();

        return persons;
    }
}