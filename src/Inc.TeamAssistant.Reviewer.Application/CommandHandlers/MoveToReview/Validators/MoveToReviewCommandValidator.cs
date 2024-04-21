using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview.Validators;

internal sealed class MoveToReviewCommandValidator : AbstractValidator<MoveToReviewCommand>
{
    public MoveToReviewCommandValidator(ReviewerOptions reviewerOptions)
    {
        ArgumentNullException.ThrowIfNull(reviewerOptions);

        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.Description)
            .NotEmpty()
            .MaximumLength(2000)
            .Must(e => !e.StartsWith("/"))
            .WithMessage("'{PropertyName}' please enter text value")
            .Must(e => reviewerOptions.LinksPrefix.Any(e.Contains))
            .WithMessage("'{PropertyName}' must contains a link to the source code");
    }
}