using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetRetroState.Validators;

internal sealed class SetRetroStateCommandValidator : AbstractValidator<SetRetroStateCommand>
{
    public SetRetroStateCommandValidator()
    {
        RuleFor(c => c.TeamId)
            .NotEmpty();
        
        RuleFor(c => c.PersonId)
            .NotEmpty();
    }
}