using System.Text;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;
using MediatR;
using Inc.TeamAssistant.Appraiser.Application.Extensions;
using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowParticipants;

internal sealed class ShowParticipantsCommandHandler : IRequestHandler<ShowParticipantsQuery, CommandResult>
{
    private readonly IAssessmentSessionRepository _repository;
    private readonly IMessageBuilder _messageBuilder;

    public ShowParticipantsCommandHandler(IAssessmentSessionRepository repository, IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(ShowParticipantsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var assessmentSession = _repository.Find(query.AppraiserId).EnsureForAppraiser(query.AppraiserName);
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.AppraiserList,
            assessmentSession.LanguageId));
        foreach (var appraiser in assessmentSession.Participants)
            messageBuilder.AppendLine(appraiser.Name);

        return CommandResult.Build(NotificationMessage.Create(query.TargetChatId, messageBuilder.ToString()));
    }
}