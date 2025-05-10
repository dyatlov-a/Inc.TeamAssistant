using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.UpdateTeam.Validators;

internal sealed class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
{
    public UpdateTeamCommandValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();
        
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}