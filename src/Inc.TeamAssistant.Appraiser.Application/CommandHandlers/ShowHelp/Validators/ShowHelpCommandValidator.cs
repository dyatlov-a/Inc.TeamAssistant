using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.ShowHelp;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ShowHelp.Validators;

internal sealed class ShowHelpCommandValidator : AbstractValidator<ShowHelpCommand>
{
    public ShowHelpCommandValidator(IValidator<IWithLanguage> languageValidator)
    {
        if (languageValidator == null)
            throw new ArgumentNullException(nameof(languageValidator));

        RuleFor(e => e).SetValidator(languageValidator);
    }
}