using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;

public sealed record MoveToNextRetroStateCommand(Guid Id)
    : IRequest;