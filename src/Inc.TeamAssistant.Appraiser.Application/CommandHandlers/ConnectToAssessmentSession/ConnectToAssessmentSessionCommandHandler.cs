using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToAssessmentSession;

internal sealed class ConnectToAssessmentSessionCommandHandler
    : IRequestHandler<ConnectToAssessmentSessionCommand, ConnectToAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation _dialogContinuation;

    public ConnectToAssessmentSessionCommandHandler(IAssessmentSessionRepository repository, IDialogContinuation dialogContinuation)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<ConnectToAssessmentSessionResult> Handle(ConnectToAssessmentSessionCommand toAssessmentSessionCommand, CancellationToken cancellationToken)
    {
        if (toAssessmentSessionCommand is null)
            throw new ArgumentNullException(nameof(toAssessmentSessionCommand));

        var existSessionAssessmentSession = _repository.Find(toAssessmentSessionCommand.AppraiserId);
        if (existSessionAssessmentSession is not null)
        {
            var messageId = existSessionAssessmentSession.Id == toAssessmentSessionCommand.AssessmentSessionId
                ? Messages.AppraiserConnectWithError
                : Messages.AppraiserConnectedToOtherSession;

            throw new AppraiserUserException(messageId, toAssessmentSessionCommand.AppraiserName, existSessionAssessmentSession.Title);
        }

        if (toAssessmentSessionCommand.AssessmentSessionId is null)
            throw new ApplicationException("AssessmentSessionId is empty.");

        var assessmentSession = _repository
			.Find(toAssessmentSessionCommand.AssessmentSessionId)
			.EnsureForAppraiser(toAssessmentSessionCommand.AppraiserName);

		assessmentSession.Connect(toAssessmentSessionCommand.AppraiserId, toAssessmentSessionCommand.AppraiserName);
        _dialogContinuation.End(toAssessmentSessionCommand.AppraiserId, ContinuationState.EnterSessionId);

        var result = new ConnectToAssessmentSessionResult(
            assessmentSession.Moderator.Id,
            assessmentSession.Id,
            assessmentSession.LanguageId,
            assessmentSession.Title,
            toAssessmentSessionCommand.AppraiserName,
            StoryInProgress: assessmentSession.CurrentStory != Story.Empty);

        return Task.FromResult(result);
    }
}