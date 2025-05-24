using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveRetroItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.RemoveRetroItem.Validators;

internal sealed class RemoveRetroItemCommandValidator : AbstractValidator<RemoveRetroItemCommand>
{
    public RemoveRetroItemCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}