using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;

public sealed record SetRetroStateCommand(
    Guid TeamId,
    long PersonId,
    bool Finished,
    bool HandRaised)
    : IRequest;