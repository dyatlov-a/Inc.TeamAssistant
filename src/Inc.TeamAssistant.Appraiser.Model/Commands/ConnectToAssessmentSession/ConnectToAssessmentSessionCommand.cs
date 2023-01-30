using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;

public sealed record ConnectToAssessmentSessionCommand(
        AssessmentSessionId? AssessmentSessionId,
        LanguageId LanguageId,
        ParticipantId AppraiserId,
        string AppraiserName)
	: IRequest<ConnectToAssessmentSessionResult>, IWithAppraiser, IWithLanguage;