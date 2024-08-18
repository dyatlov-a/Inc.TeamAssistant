using FluentValidation;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Validators;

internal sealed class GetLocationsQueryValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsQueryValidator()
    {
        RuleFor(e => e.MapId)
            .NotEmpty();
    }
}