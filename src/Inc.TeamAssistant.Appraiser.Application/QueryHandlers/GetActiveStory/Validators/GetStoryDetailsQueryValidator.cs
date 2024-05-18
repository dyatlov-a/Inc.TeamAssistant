using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetActiveStory.Validators;

internal sealed class GetStoryDetailsQueryValidator : AbstractValidator<GetActiveStoryQuery>
{
    public GetStoryDetailsQueryValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
    }
}