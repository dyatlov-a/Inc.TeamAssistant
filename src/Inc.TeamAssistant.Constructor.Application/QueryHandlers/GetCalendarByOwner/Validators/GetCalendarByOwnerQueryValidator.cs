using FluentValidation;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetCalendarByOwner.Validators;

internal sealed class GetCalendarByOwnerQueryValidator : AbstractValidator<GetCalendarByOwnerQuery>
{
    public GetCalendarByOwnerQueryValidator()
    {
        RuleFor(e => e.OwnerId)
            .NotEmpty();
    }
}