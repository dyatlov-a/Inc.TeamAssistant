using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Common.Validators;

internal sealed class LanguageValidator : AbstractValidator<IWithLanguage>
{
    private readonly IMessageService _messageService;

    public LanguageValidator(IMessageService messageService)
    {
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));

        RuleFor(e => e.LanguageId)
            .NotEmpty()
            .MustAsync(LanguageIsValid);
    }

    private async Task<bool> LanguageIsValid(LanguageId languageId, CancellationToken cancellationToken)
    {
        var languages = await _messageService.GetAll(cancellationToken);

        return languages.Result.ContainsKey(languageId.Value);
    }
}