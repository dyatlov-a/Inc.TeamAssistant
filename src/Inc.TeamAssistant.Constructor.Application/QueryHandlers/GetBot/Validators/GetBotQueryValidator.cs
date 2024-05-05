using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBot.Validators;

internal sealed class GetBotQueryValidator : AbstractValidator<GetBotQuery>
{
    public GetBotQueryValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();

        RuleFor(e => e.OwnerId)
            .NotEmpty();
    }
}