using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Commands.StartStorySelection;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.DialogContinuations;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.StartStorySelection;

internal sealed class StartStorySelectionCommandHandler : IRequestHandler<StartStorySelectionCommand, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;
    private readonly IMessageBuilder _messageBuilder;

    public StartStorySelectionCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation<ContinuationState> dialogContinuation,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(StartStorySelectionCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var assessmentSession = _repository.Find(command.ModeratorId).EnsureForModerator(command.ModeratorName);

		assessmentSession.StartStorySelection(command.ModeratorId);
        _dialogContinuation.TryBegin(command.ModeratorId, ContinuationState.EnterStory);
        
        var message = await _messageBuilder.Build(
            Messages.EnterStoryName,
            assessmentSession.LanguageId);

        return CommandResult.Build(NotificationMessage.Create(command.TargetChatId, message));
    }
}