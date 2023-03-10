using FluentValidation;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.AllowUseName;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToAssessmentSession.Validators;

internal sealed class ConnectToAssessmentSessionCommandValidator : AbstractValidator<ConnectToAssessmentSessionCommand>
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly ICommandProvider _commandProvider;

    public ConnectToAssessmentSessionCommandValidator(
        IValidator<IWithAppraiser> appraiserValidator,
        IValidator<IWithLanguage> languageValidator,
        IMessageBuilder messageBuilder,
        ICommandProvider commandProvider)
    {
        if (appraiserValidator is null)
            throw new ArgumentNullException(nameof(appraiserValidator));
        if (languageValidator is null)
            throw new ArgumentNullException(nameof(languageValidator));

        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _commandProvider = commandProvider ?? throw new ArgumentNullException(nameof(commandProvider));

        RuleFor(e => e).SetValidator(appraiserValidator);

        RuleFor(e => e).SetValidator(languageValidator);

        RuleFor(e => e.AssessmentSessionId).CustomAsync(HasAssessmentSessionId);
    }

    private async Task HasAssessmentSessionId(
        AssessmentSessionId? assessmentSessionId,
        ValidationContext<ConnectToAssessmentSessionCommand> context,
        CancellationToken cancellationToken)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var joinToSessionCommand = _commandProvider.GetCommand(typeof(JoinToAssessmentSessionCommand));
        var createAssessmentCommand = _commandProvider.GetCommand(typeof(CreateAssessmentSessionCommand));
        var allowUseUsernameCommand = _commandProvider.GetCommand(typeof(AllowUseNameCommand));
        if (assessmentSessionId is null || assessmentSessionId.Value == Guid.Empty)
            context.AddFailure(await _messageBuilder.Build(
                Messages.Error_StarterMessage,
                context.InstanceToValidate.LanguageId,
                joinToSessionCommand,
                createAssessmentCommand,
                allowUseUsernameCommand));
    }
}