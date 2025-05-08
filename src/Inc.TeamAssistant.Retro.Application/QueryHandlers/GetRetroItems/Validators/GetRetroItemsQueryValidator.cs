using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroItems.Validators;

internal sealed class GetRetroItemsQueryValidator : AbstractValidator<GetRetroItemsQuery>
{
    public GetRetroItemsQueryValidator()
    {
        RuleFor(c => c.TeamId)
            .NotEmpty();
    }
}