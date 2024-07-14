using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetAssessmentHistory.Validators;

internal sealed class GetAssessmentHistoryQueryValidator : AbstractValidator<GetAssessmentHistoryQuery>
{
    public GetAssessmentHistoryQueryValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
    }
}