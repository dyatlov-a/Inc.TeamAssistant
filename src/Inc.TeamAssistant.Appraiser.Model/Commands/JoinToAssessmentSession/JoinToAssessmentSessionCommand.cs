using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;

public sealed record JoinToAssessmentSessionCommand(
        long TargetChatId,
        ParticipantId AppraiserId,
        LanguageId LanguageId)
    : IRequest<CommandResult>, IWithLanguage;