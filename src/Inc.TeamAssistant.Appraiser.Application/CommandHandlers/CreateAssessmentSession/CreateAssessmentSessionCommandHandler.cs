using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.DialogContinuations;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession;

internal sealed class CreateAssessmentSessionCommandHandler
    : IRequestHandler<CreateAssessmentSessionCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;
    private readonly IAssessmentSessionMetrics _metrics;
    private readonly IEnumerable<LanguageContext> _languageContextList;
    private readonly IMessageBuilder _messageBuilder;

    public CreateAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation<ContinuationState> dialogContinuation,
        IAssessmentSessionMetrics metrics,
        IEnumerable<LanguageContext> languageContextList,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        _languageContextList = languageContextList ?? throw new ArgumentNullException(nameof(languageContextList));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(CreateAssessmentSessionCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var existsSession = _repository.Find(command.ModeratorId);
        var isNotExists = existsSession is null;

        if (isNotExists)
        {
            var moderator = new Participant(command.ModeratorId, command.ModeratorName);
            var assessmentSession = new AssessmentSession(command.TargetChatId, moderator, command.LanguageId);

            _dialogContinuation.TryBegin(command.ModeratorId, ContinuationState.EnterTitle);
            _repository.Add(assessmentSession);

            _metrics.Created();
        }

        var otherLanguages = _languageContextList.Where(c => c.LanguageId != command.LanguageId);
        var setLanguageCommands = string.Join(' ', otherLanguages.Select(l => l.Command));

        if (isNotExists)
        {
            var enterSessionNameMessage = await _messageBuilder.Build(
                Messages.EnterSessionName,
                command.LanguageId,
                setLanguageCommands);
            return CommandResult.Build(NotificationMessage.Create(command.TargetChatId, enterSessionNameMessage));
        }

        var createAssessmentSessionFailedMessage = await _messageBuilder.Build(
            Messages.CreateAssessmentSessionFailed,
            command.LanguageId);
        return CommandResult.Build(NotificationMessage.Create(
            command.TargetChatId,
            createAssessmentSessionFailedMessage));
    }
}