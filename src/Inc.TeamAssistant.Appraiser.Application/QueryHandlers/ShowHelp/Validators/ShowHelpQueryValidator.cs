using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowHelp.Validators;

internal sealed class ShowHelpQueryValidator : AbstractValidator<ShowHelpQuery>
{
    public ShowHelpQueryValidator(IValidator<IWithLanguage> languageValidator)
    {
        if (languageValidator == null)
            throw new ArgumentNullException(nameof(languageValidator));

        RuleFor(e => e).SetValidator(languageValidator);
    }
}