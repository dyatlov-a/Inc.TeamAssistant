using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishAssessmentSession.Validators;

internal sealed class FinishAssessmentSessionCommandValidator : AbstractValidator<FinishAssessmentSessionCommand>
{
    public FinishAssessmentSessionCommandValidator(IValidator<IWithModerator> moderatorValidator)
    {
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));

        RuleFor(e => e).SetValidator(moderatorValidator);
    }
}