using FluentValidation;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItemsHistory;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetActionItemsHistory.Validators;

internal sealed class GetActionItemsHistoryQueryValidator : AbstractValidator<GetActionItemsHistoryQuery>
{
    public GetActionItemsHistoryQueryValidator()
    {
        RuleFor(q => q.RoomId)
            .NotEmpty();
        
        RuleFor(q => q.State)
            .Must(s => Enum.TryParse<ActionItemState>(s, ignoreCase: true, out _))
            .WithMessage("Invalid action item state.");

        RuleFor(q => q.Offset)
            .GreaterThanOrEqualTo(0);

        RuleFor(q => q.Limit)
            .GreaterThan(0);
    }
}