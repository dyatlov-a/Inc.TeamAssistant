using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetWidgets.Validators;

internal sealed class GetWidgetsQueryValidator : AbstractValidator<GetWidgetsQuery>
{
    public GetWidgetsQueryValidator()
    {
        RuleFor(e => e.BotId)
            .NotEmpty();
    }
}