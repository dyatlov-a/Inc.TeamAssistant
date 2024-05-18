using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetStories.Validators;

internal sealed class GetStoriesQueryValidator : AbstractValidator<GetStoriesQuery>
{
    public GetStoriesQueryValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.AssessmentDate)
            .NotEmpty();
    }
}