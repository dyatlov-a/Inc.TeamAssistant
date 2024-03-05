using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept.Validators;

internal sealed class MoveToAcceptCommandValidator : AbstractValidator<MoveToAcceptCommand>
{
    public MoveToAcceptCommandValidator()
    {
        RuleFor(e => e.TaskId)
            .NotEmpty();
    }
}