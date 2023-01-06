using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession.Validators;

internal sealed class CreateAssessmentSessionCommandValidator : AbstractValidator<CreateAssessmentSessionCommand>
{
    public CreateAssessmentSessionCommandValidator(
        IValidator<IWithLanguage> languageValidator,
        IValidator<IWithModerator> moderatorValidator)
    {
        if (languageValidator is null)
            throw new ArgumentNullException(nameof(languageValidator));
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));

        RuleFor(e => e).SetValidator(languageValidator);

        RuleFor(e => e).SetValidator(moderatorValidator);
    }
}