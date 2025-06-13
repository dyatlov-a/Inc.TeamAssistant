using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.GiveFacilitator;

public sealed record GiveFacilitatorCommand(
    Guid RoomId,
    Guid? RetroSessionId)
    : IRequest;