using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ChangeLanguage.Validators;

internal sealed class ChangeLanguageCommandValidator : AbstractValidator<ChangeLanguageCommand>
{
    public ChangeLanguageCommandValidator(
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