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
            .Must(e => HasDescriptionAndLinks(e, reviewerOptions))
            .WithMessage("'{PropertyName}' must contains a link to the source code and some description");
    }

    private static bool HasDescriptionAndLinks(string description, ReviewerOptions reviewerOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(description));
        ArgumentNullException.ThrowIfNull(nameof(reviewerOptions));
        
        var descriptionParts = description.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var links = descriptionParts.Where(t => reviewerOptions.LinksPrefix.Any(t.Contains)).ToArray();

        return links.Any() && descriptionParts.Length > links.Length;
    }
}