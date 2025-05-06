using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroCardPool;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.CreateRetroCardPool.Validators;

internal sealed class CreateRetroCardPoolCommandValidator : AbstractValidator<CreateRetroCardPoolCommand>
{
    public CreateRetroCardPoolCommandValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}