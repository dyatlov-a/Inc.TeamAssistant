using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;

public sealed record FinishAssessmentSessionCommand(long TargetChatId, long ModeratorId, string ModeratorName)
    : IRequest<CommandResult>, IWithModerator;