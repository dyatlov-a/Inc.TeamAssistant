using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.UpdateRoom.Validators;

internal sealed class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();
        
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}