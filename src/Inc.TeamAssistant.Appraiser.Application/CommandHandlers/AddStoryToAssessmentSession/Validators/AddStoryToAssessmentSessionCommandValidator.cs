using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession.Validators;

internal sealed class AddStoryToAssessmentSessionCommandValidator
    : AbstractValidator<AddStoryToAssessmentSessionCommand>
{
    private readonly AddStoryToAssessmentSessionOptions _toAssessmentSessionOptions;
    private readonly IMessageBuilder _messageBuilder;
    private readonly IAssessmentSessionRepository _repository;

    public AddStoryToAssessmentSessionCommandValidator(
        AddStoryToAssessmentSessionOptions toAssessmentSessionOptions,
        IValidator<IWithModerator> moderatorValidator,
        IMessageBuilder messageBuilder,
        IAssessmentSessionRepository repository,
        IValidator<IWithLanguage> languageValidator)
    {
        if (moderatorValidator is null)
            throw new ArgumentNullException(nameof(moderatorValidator));
        if (languageValidator is null)
            throw new ArgumentNullException(nameof(languageValidator));

        _toAssessmentSessionOptions = toAssessmentSessionOptions ?? throw new ArgumentNullException(nameof(toAssessmentSessionOptions));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        RuleFor(e => e.LanguageId).NotEmpty();

        RuleFor(e => e).SetValidator(moderatorValidator);

        RuleFor(e => e).SetValidator(languageValidator);

        RuleFor(e => e).CustomAsync(CheckCommand);
    }

    private async Task CheckCommand(
        AddStoryToAssessmentSessionCommand toAssessmentSessionCommand,
        ValidationContext<AddStoryToAssessmentSessionCommand> context,
        CancellationToken cancellationToken)
    {
        if (toAssessmentSessionCommand is null)
            throw new ArgumentNullException(nameof(toAssessmentSessionCommand));
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var assessmentSession = _repository.Find(toAssessmentSessionCommand.ModeratorId);
        var currentLanguageId = assessmentSession?.LanguageId ?? toAssessmentSessionCommand.LanguageId;

        if (string.IsNullOrWhiteSpace(toAssessmentSessionCommand.Title))
        {
            context.AddFailure(await _messageBuilder.Build(Messages.Error_StoryTitleIsEmpty, currentLanguageId));
            return;
        }

        foreach (var link in toAssessmentSessionCommand.Links)
        {
            if (!UrlFormatIsValid(link))
            {
                context.AddFailure(await _messageBuilder.Build(Messages.Error_StoryLinkFormat, currentLanguageId, link));
                return;
            }
        }
    }

    private bool UrlFormatIsValid(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        const int urlMinLength = 11;
        if (url.Length < urlMinLength)
            return false;

        return _toAssessmentSessionOptions.LinksPrefix.Any(i => url.StartsWith(i, StringComparison.InvariantCultureIgnoreCase));
    }
}