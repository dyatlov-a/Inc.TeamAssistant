using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline.Validators;

internal sealed class MoveToDeclineCommandValidator : AbstractValidator<MoveToDeclineCommand>
{
    public MoveToDeclineCommandValidator()
    {
        RuleFor(e => e.TaskId)
            .NotEmpty();
    }
}