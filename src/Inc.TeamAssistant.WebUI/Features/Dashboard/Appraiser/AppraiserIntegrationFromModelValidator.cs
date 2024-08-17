using FluentValidation;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public sealed class AppraiserIntegrationFromModelValidator : AbstractValidator<AppraiserIntegrationFromModel>
{
    public AppraiserIntegrationFromModelValidator()
    {
        RuleFor(e => e.AccessToken)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(e => e.ProjectKey)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(e => e.ScrumMasterId)
            .NotEmpty();
    }
}