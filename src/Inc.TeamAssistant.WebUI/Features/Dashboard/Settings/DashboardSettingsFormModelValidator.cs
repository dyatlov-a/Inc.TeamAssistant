using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Settings;

internal sealed class DashboardSettingsFormModelValidator : AbstractValidator<DashboardSettingsFormModel>
{
    public DashboardSettingsFormModelValidator()
    {
        RuleFor(e => e.Items)
            .NotEmpty();

        RuleForEach(e => e.Items)
            .ChildRules(e =>
            {
                e.RuleFor(c => c.Type)
                    .NotEmpty();

                e.RuleFor(c => c.Position)
                    .GreaterThan(0);
            });
    }
}