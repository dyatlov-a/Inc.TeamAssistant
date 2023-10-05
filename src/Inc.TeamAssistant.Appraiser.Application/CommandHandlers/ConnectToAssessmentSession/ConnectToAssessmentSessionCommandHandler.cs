using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.DialogContinuations;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToAssessmentSession;

internal sealed class ConnectToAssessmentSessionCommandHandler
    : IRequestHandler<ConnectToAssessmentSessionCommand, ConnectToAssessmentSessionResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IDialogContinuation<ContinuationState> _dialogContinuation;

    public ConnectToAssessmentSessionCommandHandler(
        IAssessmentSessionRepository repository,
        IDialogContinuation<ContinuationState> dialogContinuation)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
    }

    public Task<ConnectToAssessmentSessionResult> Handle(
        ConnectToAssessmentSessionCommand command,
        CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var existSessionAssessmentSession = _repository.Find(command.AppraiserId);
        if (existSessionAssessmentSession is not null)
        {
            if (existSessionAssessmentSession.Participants.Any(p => p.Id == command.AppraiserId))
            {
                var messageId = existSessionAssessmentSession.Id == command.AssessmentSessionId
                    ? Messages.AppraiserConnectWithError
                    : Messages.AppraiserConnectedToOtherSession;

                throw new AppraiserUserException(messageId, command.AppraiserName, existSessionAssessmentSession.Title);
            }

            if (existSessionAssessmentSession.Moderator.Id == command.AppraiserId
                && existSessionAssessmentSession.Id != command.AssessmentSessionId)
                throw new AppraiserUserException(Messages.AppraiserConnectedToOtherSession, command.AppraiserName, existSessionAssessmentSession.Title);
        }
        
        // TODO: remove nullable check
        if (command.AssessmentSessionId is null)
            throw new ApplicationException("AssessmentSessionId is empty.");
        var assessmentSession = _repository
			.Find(command.AssessmentSessionId)
			.EnsureForAppraiser(command.AppraiserName);

		assessmentSession.Connect(command.AppraiserId, command.AppraiserName);
        _dialogContinuation.End(command.AppraiserId.Value, ContinuationState.EnterSessionId);

        var result = new ConnectToAssessmentSessionResult(
            assessmentSession.Moderator.Id,
            command.AppraiserName,
            IsModerator: command.AppraiserId != assessmentSession.Moderator.Id,
            assessmentSession.Title,
            assessmentSession.InProgress(),
            SummaryByStoryConverter.ConvertTo(assessmentSession));

        return Task.FromResult(result);
    }
}