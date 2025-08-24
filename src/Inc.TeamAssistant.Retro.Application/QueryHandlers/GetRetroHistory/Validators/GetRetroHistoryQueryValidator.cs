using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroHistory;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroHistory.Validators;

internal sealed class GetRetroHistoryQueryValidator : AbstractValidator<GetRetroHistoryQuery>
{
    public GetRetroHistoryQueryValidator()
    {
        RuleFor(q => q.RetroSessionId)
            .NotEmpty();
    }
}