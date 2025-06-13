using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;

public sealed record SetRetroStateCommand(
    Guid RoomId,
    long PersonId,
    bool Finished,
    bool HandRaised)
    : IRequest;