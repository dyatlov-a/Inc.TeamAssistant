using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetActionItems.Validators;

internal sealed class GetActionItemsQueryValidator : AbstractValidator<GetActionItemsQuery>
{
    public GetActionItemsQueryValidator()
    {
        RuleFor(q => q.RoomId)
            .NotEmpty();

        RuleFor(q => q.Limit)
            .GreaterThan(0);
    }
}