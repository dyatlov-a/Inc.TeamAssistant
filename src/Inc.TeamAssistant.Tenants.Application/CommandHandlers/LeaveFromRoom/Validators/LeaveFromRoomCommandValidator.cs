using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.LeaveFromRoom;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.LeaveFromRoom.Validators;

internal sealed class LeaveFromRoomCommandValidator : AbstractValidator<LeaveFromRoomCommand>
{
    public LeaveFromRoomCommandValidator()
    {
        RuleFor(c => c.ConnectionId)
            .NotEmpty();
        
        RuleFor(c => c.RoomId)
            .NotEmpty();
    }
}