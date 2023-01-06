using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ExitFromAssessmentSession.Validators;

internal sealed class ExitFromAssessmentSessionCommandValidator : AbstractValidator<ExitFromAssessmentSessionCommand>
{
    public ExitFromAssessmentSessionCommandValidator(IValidator<IWithAppraiser> appraiserValidator)
    {
        if (appraiserValidator is null)
            throw new ArgumentNullException(nameof(appraiserValidator));

        RuleFor(e => e).SetValidator(appraiserValidator);
    }
}