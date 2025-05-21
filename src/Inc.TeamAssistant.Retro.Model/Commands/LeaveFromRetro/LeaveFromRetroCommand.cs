using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.LeaveFromRetro;

public sealed record LeaveFromRetroCommand(string ConnectionId, Guid TeamId)
    : IRequest;