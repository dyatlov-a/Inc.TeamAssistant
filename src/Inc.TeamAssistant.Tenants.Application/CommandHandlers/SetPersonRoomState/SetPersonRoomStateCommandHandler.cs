using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;
using Inc.TeamAssistant.Tenants.Model.Commands.SetPersonRoomState;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.SetPersonRoomState;

internal sealed class SetPersonRoomStateCommandHandler : IRequestHandler<SetPersonRoomStateCommand>
{
    private readonly IPersonRoomState _personRoomState;
    private readonly IRoomEventSender _eventSender;

    public SetPersonRoomStateCommandHandler(IPersonRoomState personRoomState, IRoomEventSender eventSender)
    {
        _personRoomState = personRoomState ?? throw new ArgumentNullException(nameof(personRoomState));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(SetPersonRoomStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var ticket = new PersonRoomTicket(command.PersonId, command.Finished, command.HandRaised);
        
        _personRoomState.Set(command.RoomId, ticket);

        await _eventSender.RetroStateChanged(
            command.RoomId,
            command.PersonId,
            command.Finished,
            command.HandRaised);
    }
}