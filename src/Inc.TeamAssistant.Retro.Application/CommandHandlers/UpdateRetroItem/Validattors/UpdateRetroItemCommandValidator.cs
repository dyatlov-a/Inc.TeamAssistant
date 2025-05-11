using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.UpdateRetroItem.Validattors;

internal sealed class UpdateRetroItemCommandValidator : AbstractValidator<UpdateRetroItemCommand>
{
    public UpdateRetroItemCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}