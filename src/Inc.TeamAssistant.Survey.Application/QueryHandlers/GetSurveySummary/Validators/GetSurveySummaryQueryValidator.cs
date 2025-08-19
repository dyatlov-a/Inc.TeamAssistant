using FluentValidation;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveySummary.Validators;

internal sealed class GetSurveySummaryQueryValidator : AbstractValidator<GetSurveySummaryQuery>
{
    public GetSurveySummaryQueryValidator()
    {
        RuleFor(q => q.RoomId)
            .NotEmpty();

        RuleFor(q => q.Limit)
            .GreaterThan(0);
    }
}