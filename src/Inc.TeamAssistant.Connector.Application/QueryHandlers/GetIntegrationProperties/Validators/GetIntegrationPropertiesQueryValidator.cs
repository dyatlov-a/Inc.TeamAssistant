using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetIntegrationProperties.Validators;

internal sealed class GetIntegrationPropertiesQueryValidator : AbstractValidator<GetIntegrationPropertiesQuery>
{
    public GetIntegrationPropertiesQueryValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
    }
}