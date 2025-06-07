using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroAssessment.Validators;

internal sealed class GetRetroAssessmentQueryValidator : AbstractValidator<GetRetroAssessmentQuery>
{
    public GetRetroAssessmentQueryValidator()
    {
        RuleFor(q => q.SessionId)
            .NotEmpty();
    }
}