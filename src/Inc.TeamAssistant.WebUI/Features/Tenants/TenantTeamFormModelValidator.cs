using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Tenants;

public sealed class TenantTeamFormModelValidator : AbstractValidator<TenantTeamFormModel>
{
    public TenantTeamFormModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}