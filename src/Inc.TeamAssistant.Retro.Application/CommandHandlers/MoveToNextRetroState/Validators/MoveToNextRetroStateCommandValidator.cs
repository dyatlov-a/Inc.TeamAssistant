using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.MoveToNextRetroState;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.MoveToNextRetroState.Validators;

internal sealed class MoveToNextRetroStateCommandValidator : AbstractValidator<MoveToNextRetroStateCommand>
{
    public MoveToNextRetroStateCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}