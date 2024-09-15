using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetTeammates.Validators;

internal sealed class GetTeammatesQueryValidator : AbstractValidator<GetTeammatesQuery>
{
    public GetTeammatesQueryValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();
    }
}