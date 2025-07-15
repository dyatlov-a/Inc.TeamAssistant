using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.SetPersonRoomState;

public sealed record SetPersonRoomStateCommand(
    Guid RoomId,
    long PersonId,
    bool Finished,
    bool HandRaised)
    : IRequest;