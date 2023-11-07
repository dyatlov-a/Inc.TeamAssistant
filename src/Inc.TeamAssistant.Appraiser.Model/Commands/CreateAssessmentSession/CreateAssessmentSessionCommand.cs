using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;

public sealed record CreateAssessmentSessionCommand(
        long TargetChatId,
        ParticipantId ModeratorId,
        string ModeratorName,
        LanguageId LanguageId)
	: IRequest<CommandResult>, IWithModerator, IWithLanguage;