using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.ReassignReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.ReassignReview.Validators;

internal sealed class ReassignReviewCommandValidator : AbstractValidator<ReassignReviewCommand>
{
    public ReassignReviewCommandValidator()
    {
        RuleFor(e => e.TaskId)
            .NotEmpty();
    }
}