using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.LeaveFromRooms;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.LeaveFromRooms.Validators;

internal sealed class LeaveFromRoomsCommandValidator : AbstractValidator<LeaveFromRoomsCommand>
{
    public LeaveFromRoomsCommandValidator()
    {
        RuleFor(c => c.ConnectionId)
            .NotEmpty();
    }
}