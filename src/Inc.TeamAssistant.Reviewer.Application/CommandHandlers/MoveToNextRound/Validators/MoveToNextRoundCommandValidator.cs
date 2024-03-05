using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound.Validators;

internal sealed class MoveToNextRoundCommandValidator : AbstractValidator<MoveToNextRoundCommand>
{
    public MoveToNextRoundCommandValidator()
    {
        RuleFor(e => e.TaskId)
            .NotEmpty();
    }
}