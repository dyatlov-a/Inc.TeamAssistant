using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Validators;

internal sealed class LanguageValidator : AbstractValidator<IWithLanguage>
{
    private readonly IMessageProvider _messageProvider;

    public LanguageValidator(IMessageProvider messageProvider)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));

        RuleFor(e => e.LanguageId)
            .NotEmpty()
            .MustAsync(LanguageIsValid);
    }

    private async Task<bool> LanguageIsValid(LanguageId languageId, CancellationToken cancellationToken)
    {
        var languages = await _messageProvider.Get();

        return languages.Result.ContainsKey(languageId.Value);
    }
}