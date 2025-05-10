using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.CreateTeam.Validators;

internal sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}