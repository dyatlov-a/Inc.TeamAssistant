using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Validators;

internal sealed class MoveToReviewCommandValidator : AbstractValidator<MoveToReviewCommand>
{
    public MoveToReviewCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.Description)
            .NotEmpty()
            .MaximumLength(2000)
            .Must(e => !e.StartsWith("/"))
            .WithMessage("Please enter text value.");
    }
}