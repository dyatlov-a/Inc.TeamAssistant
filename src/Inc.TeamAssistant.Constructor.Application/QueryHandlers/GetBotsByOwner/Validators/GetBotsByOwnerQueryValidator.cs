using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBotsByOwner.Validators;

internal sealed class GetBotsByOwnerQueryValidator : AbstractValidator<GetBotsByOwnerQuery>
{
    public GetBotsByOwnerQueryValidator()
    {
        RuleFor(e => e.OwnerId).NotEmpty();
    }
}