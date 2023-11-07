using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;

public sealed record FinishAssessmentSessionCommand(long TargetChatId, ParticipantId ModeratorId, string ModeratorName)
    : IRequest<CommandResult>, IWithModerator;