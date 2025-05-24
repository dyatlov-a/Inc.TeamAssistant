using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.JoinToRetro;

public sealed record JoinToRetroCommand(string ConnectionId, Guid TeamId)
    : IRequest;