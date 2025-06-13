using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.GiveFacilitator;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.GiveFacilitator.Validators;

internal sealed class GiveFacilitatorCommandValidator : AbstractValidator<GiveFacilitatorCommand>
{
    public GiveFacilitatorCommandValidator()
    {
        RuleFor(c => c.RoomId)
            .NotEmpty();
        
        RuleFor(c => c.RetroSessionId)
            .NotEmpty()
            .When(c => c.RetroSessionId.HasValue);
    }
}