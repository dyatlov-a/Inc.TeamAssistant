using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.UpdateRetroItem.Validators;

internal sealed class UpdateRetroItemCommandValidator : AbstractValidator<UpdateRetroItemCommand>
{
    public UpdateRetroItemCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();

        RuleFor(c => c.ParentId)
            .NotEmpty()
            .When(c => c.ParentId.HasValue);
        
        RuleFor(c => c.ColumnId)
            .NotEmpty();
        
        RuleFor(c => c.Position)
            .NotEmpty();
    }
}