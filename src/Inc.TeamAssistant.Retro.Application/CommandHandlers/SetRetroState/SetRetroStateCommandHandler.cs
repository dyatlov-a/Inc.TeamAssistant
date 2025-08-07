using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetRetroState;

internal sealed class SetRetroStateCommandHandler : IRequestHandler<SetRetroStateCommand>
{
    private readonly IPersonState _personState;
    private readonly IRetroEventSender _eventSender;
    private readonly ITeamAccessor _teamAccessor;

    public SetRetroStateCommandHandler(
        IPersonState personState,
        IRetroEventSender eventSender,
        ITeamAccessor teamAccessor)
    {
        _personState = personState ?? throw new ArgumentNullException(nameof(personState));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(SetRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = await _teamAccessor.EnsurePerson(command.PersonId, token);
        var ticket = new PersonStateTicket(person, command.Finished, command.HandRaised);
        
        _personState.Set(RoomId.CreateForRetro(command.RoomId), ticket);

        await _eventSender.RetroStateChanged(
            command.RoomId,
            command.PersonId,
            command.Finished,
            command.HandRaised);
    }
}