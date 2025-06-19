using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.CreateRoom.Validators;

internal sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}