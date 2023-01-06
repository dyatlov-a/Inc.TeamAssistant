using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;

public sealed record CreateAssessmentSessionCommand(
        long ChatId,
        ParticipantId ModeratorId,
        string ModeratorName,
        LanguageId LanguageId)
	: IRequest<CreateAssessmentSessionResult>, IWithModerator, IWithLanguage;