using FluentValidation;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeActionItem.Validators;

internal sealed class ChangeActionItemCommandValidator : AbstractValidator<ChangeActionItemCommand>
{
    public ChangeActionItemCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();

        RuleFor(e => e.RetroItemId)
            .NotEmpty();
        
        RuleFor(e => e.TeamIdForNotify)
            .NotEmpty();

        RuleFor(e => e.Text)
            .NotEmpty();

        RuleFor(e => e.State)
            .Must(e => Enum.TryParse<ActionItemState>(e, ignoreCase: true, out _))
            .WithMessage("Invalid action item state.")
            .When(e => !string.IsNullOrWhiteSpace(e.State));
    }
}