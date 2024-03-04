using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.ChangeTeamProperty.Validators;

internal sealed class ChangeTeamPropertyCommandValidator : AbstractValidator<ChangeTeamPropertyCommand>
{
    public ChangeTeamPropertyCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.Name)
            .NotEmpty();
        
        RuleFor(e => e.Value)
            .NotEmpty();
    }
}