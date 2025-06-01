using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeActionItem.Validators;

internal sealed class ChangeActionItemCommandValidator : AbstractValidator<ChangeActionItemCommand>
{
    public ChangeActionItemCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.Id)
            .NotEmpty()
            .When(e => e.Id.HasValue);

        RuleFor(e => e.RetroItemId)
            .NotEmpty();

        RuleFor(e => e.Text)
            .NotEmpty();
    }
}