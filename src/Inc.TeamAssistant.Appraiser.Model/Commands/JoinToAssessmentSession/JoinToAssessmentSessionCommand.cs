using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;

public sealed record JoinToAssessmentSessionCommand(ParticipantId AppraiserId, LanguageId LanguageId)
    : IRequest<JoinToAssessmentSessionResult>, IWithLanguage;