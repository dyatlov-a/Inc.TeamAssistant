using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

internal sealed class AppraiserIntegrationFromModelValidator : AbstractValidator<AppraiserIntegrationFromModel>
{
    public AppraiserIntegrationFromModelValidator()
    {
        RuleFor(e => e.ProjectKey)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(e => e.ScrumMasterId)
            .NotEmpty();
    }
}