using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.RemoveRoom;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.RemoveRoom.Validators;

internal sealed class RemoveRoomCommandValidator : AbstractValidator<RemoveRoomCommand>
{
    public RemoveRoomCommandValidator()
    {
        RuleFor(c => c.RoomId)
            .NotEmpty();
    }
}