using FluentValidation;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetMaps.Validators;

internal sealed class GetMapsQueryValidator : AbstractValidator<GetMapsQuery>
{
    public GetMapsQueryValidator()
    {
        RuleFor(e => e.BotId)
            .NotEmpty();
    }
}