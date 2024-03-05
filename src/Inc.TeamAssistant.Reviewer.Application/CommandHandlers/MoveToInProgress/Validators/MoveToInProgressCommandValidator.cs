using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress.Validators;

internal sealed class MoveToInProgressCommandValidator : AbstractValidator<MoveToInProgressCommand>
{
    public MoveToInProgressCommandValidator()
    {
        RuleFor(e => e.TaskId)
            .NotEmpty();
    }
}