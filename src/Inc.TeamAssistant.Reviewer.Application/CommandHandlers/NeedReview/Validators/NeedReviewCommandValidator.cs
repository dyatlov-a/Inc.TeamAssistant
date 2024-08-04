using FluentValidation;
using Inc.TeamAssistant.Reviewer.Model.Commands.NeedReview;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.NeedReview.Validators;

internal sealed class NeedReviewCommandValidator : AbstractValidator<NeedReviewCommand>
{
    public NeedReviewCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.Description)
            .NotEmpty()
            .MaximumLength(2000)
            .Must(e => !e.StartsWith("/"))
            .WithMessage("'{PropertyName}' please enter text value");
    }
}