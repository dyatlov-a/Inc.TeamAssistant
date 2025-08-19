using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Inc.TeamAssistant.WebUI.Features.Survey;

internal sealed class AnswerFromModelValidator : AbstractValidator<AnswerFromModel>
{
    public AnswerFromModelValidator(IStringLocalizer<SurveyResources> localizer)
    {
        ArgumentNullException.ThrowIfNull(localizer);
        
        RuleFor(f => f.Values)
            .Must(v => v.Any())
            .WithMessage(localizer["RequiredValues"]);

        RuleFor(f => f.Comment)
            .MaximumLength(250);
    }
}