using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.ActivateAssessment;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ActivateAssessment.Validators;

internal sealed class ActivateAssessmentCommandValidator : AbstractValidator<ActivateAssessmentCommand>
{
    public ActivateAssessmentCommandValidator(IValidator<IWithModerator> moderatorValidator)
    {
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));

        RuleFor(e => e).SetValidator(moderatorValidator);

        RuleFor(e => e.Title).NotEmpty();
    }
}