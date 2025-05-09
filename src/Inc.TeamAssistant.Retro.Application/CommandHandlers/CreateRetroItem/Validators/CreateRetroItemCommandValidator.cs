using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.CreateRetroItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.CreateRetroItem.Validators;

internal sealed class CreateRetroItemCommandValidator : AbstractValidator<CreateRetroItemCommand>
{
    public CreateRetroItemCommandValidator()
    {
        RuleFor(c => c.TeamId)
            .NotEmpty();
        
        RuleFor(c => c.Type)
            .NotEmpty();
        
        RuleFor(c => c.Text)
            .MaximumLength(1000);
    }
}