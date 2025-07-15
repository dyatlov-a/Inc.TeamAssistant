using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.SetPersonRoomState;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.SetPersonRoomState.Validators;

internal sealed class SetPersonRoomStateCommandValidator : AbstractValidator<SetPersonRoomStateCommand>
{
    public SetPersonRoomStateCommandValidator()
    {
        RuleFor(c => c.RoomId)
            .NotEmpty();
        
        RuleFor(c => c.PersonId)
            .NotEmpty();
    }
}