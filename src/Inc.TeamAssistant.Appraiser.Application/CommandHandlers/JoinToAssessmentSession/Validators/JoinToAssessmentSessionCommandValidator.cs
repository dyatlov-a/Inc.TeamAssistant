using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.JoinToAssessmentSession.Validators;

internal sealed class JoinToAssessmentSessionCommandValidator : AbstractValidator<JoinToAssessmentSessionCommand>
{
    public JoinToAssessmentSessionCommandValidator(IValidator<IWithLanguage> languageValidator)
    {
        if (languageValidator is null)
            throw new ArgumentNullException(nameof(languageValidator));

        RuleFor(e => e).SetValidator(languageValidator);
    }
}