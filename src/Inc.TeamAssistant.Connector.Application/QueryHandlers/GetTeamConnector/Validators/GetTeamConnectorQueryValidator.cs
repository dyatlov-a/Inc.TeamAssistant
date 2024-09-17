using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetTeamConnector.Validators;

internal sealed class GetTeamConnectorQueryValidator : AbstractValidator<GetTeamConnectorQuery>
{
    public GetTeamConnectorQueryValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.Foreground)
            .NotEmpty();

        RuleFor(x => x.Background)
            .NotEmpty();
    }
}