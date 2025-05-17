using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroState.Validators;

internal sealed class GetRetroStateQueryValidator : AbstractValidator<GetRetroStateQuery>
{
    public GetRetroStateQueryValidator()
    {
        RuleFor(c => c.TeamId)
            .NotEmpty();
    }
}