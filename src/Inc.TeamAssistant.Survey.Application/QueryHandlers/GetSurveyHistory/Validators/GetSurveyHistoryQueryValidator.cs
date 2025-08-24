using FluentValidation;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyHistory;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveyHistory.Validators;

internal sealed class GetSurveyHistoryQueryValidator : AbstractValidator<GetSurveyHistoryQuery>
{
    public GetSurveyHistoryQueryValidator()
    {
        RuleFor(q => q.SurveyId)
            .NotEmpty();

        RuleFor(q => q.Limit)
            .GreaterThan(0);
    }
}