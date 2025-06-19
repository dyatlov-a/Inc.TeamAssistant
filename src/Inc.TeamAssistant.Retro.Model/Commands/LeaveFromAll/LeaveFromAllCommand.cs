using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.LeaveFromAll;

public sealed record LeaveFromAllCommand(string ConnectionId)
    : IRequest<LeaveFromAllResult>;