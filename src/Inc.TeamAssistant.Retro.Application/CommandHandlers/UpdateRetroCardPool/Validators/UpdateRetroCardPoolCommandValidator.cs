using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroCardPool;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.UpdateRetroCardPool.Validators;

internal sealed class UpdateRetroCardPoolCommandValidator : AbstractValidator<UpdateRetroCardPoolCommand>
{
    public UpdateRetroCardPoolCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();
        
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}