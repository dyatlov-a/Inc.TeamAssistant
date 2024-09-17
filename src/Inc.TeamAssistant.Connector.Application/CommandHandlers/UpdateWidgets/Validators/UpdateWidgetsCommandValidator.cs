using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.UpdateWidgets;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.UpdateWidgets.Validators;

internal sealed class UpdateWidgetsCommandValidator : AbstractValidator<UpdateWidgetsCommand>
{
    public UpdateWidgetsCommandValidator()
    {
        RuleFor(x => x.BotId)
            .NotEmpty();

        RuleFor(x => x.Widgets)
            .NotEmpty();

        RuleForEach(x => x.Widgets)
            .ChildRules(x =>
            {
                x.RuleFor(c => c.Key)
                    .NotEmpty();

                x.RuleFor(c => c.Value.Position)
                    .GreaterThan(0);
            });
    }
}