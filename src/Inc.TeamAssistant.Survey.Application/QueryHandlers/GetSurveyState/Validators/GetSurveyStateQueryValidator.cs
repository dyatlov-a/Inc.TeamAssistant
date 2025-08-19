using FluentValidation;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveyState.Validators;

internal sealed class GetSurveyStateQueryValidator : AbstractValidator<GetSurveyStateQuery>
{
    public GetSurveyStateQueryValidator()
    {
        RuleFor(q => q.RoomId)
            .NotEmpty();
    }
}