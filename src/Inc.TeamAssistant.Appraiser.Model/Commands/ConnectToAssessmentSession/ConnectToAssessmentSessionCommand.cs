using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;

public sealed record ConnectToAssessmentSessionCommand(
		long TargetChatId,
        Guid? AssessmentSessionId,
        LanguageId LanguageId,
		long AppraiserId,
        string AppraiserName)
	: IRequest<CommandResult>, IWithAppraiser, IWithLanguage;