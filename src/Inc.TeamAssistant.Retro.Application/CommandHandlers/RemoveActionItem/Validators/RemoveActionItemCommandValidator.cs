using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.RemoveActionItem.Validators;

internal sealed class RemoveActionItemCommandValidator : AbstractValidator<RemoveActionItemCommand>
{
    public RemoveActionItemCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();
        
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.ConnectionId)
            .NotEmpty();
    }
}