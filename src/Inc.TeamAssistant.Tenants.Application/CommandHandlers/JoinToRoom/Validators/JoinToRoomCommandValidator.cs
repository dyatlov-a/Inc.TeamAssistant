using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.JoinToRoom;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.JoinToRoom.Validators;

internal sealed class JoinToRoomCommandValidator : AbstractValidator<JoinToRoomCommand>
{
    public JoinToRoomCommandValidator()
    {
        RuleFor(c => c.ConnectionId)
            .NotEmpty();
        
        RuleFor(c => c.RoomId)
            .NotEmpty();
    }
}