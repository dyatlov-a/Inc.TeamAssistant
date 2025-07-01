using FluentValidation;
using Inc.TeamAssistant.Survey.Model.Queries.GetPersonSurvey;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetPersonSurvey.Validators;

internal sealed class GetPersonSurveyQueryValidator : AbstractValidator<GetPersonSurveyQuery>
{
    public GetPersonSurveyQueryValidator()
    {
        RuleFor(q => q.SurveyId)
            .NotEmpty();
    }
}