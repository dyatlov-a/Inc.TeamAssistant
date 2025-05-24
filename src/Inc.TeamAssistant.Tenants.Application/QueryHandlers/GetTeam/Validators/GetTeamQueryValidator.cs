using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Queries.GetTeam;

namespace Inc.TeamAssistant.Tenants.Application.QueryHandlers.GetTeam.Validators;

internal sealed class GetTeamQueryValidator : AbstractValidator<GetTeamQuery>
{
    public GetTeamQueryValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();
    }
}