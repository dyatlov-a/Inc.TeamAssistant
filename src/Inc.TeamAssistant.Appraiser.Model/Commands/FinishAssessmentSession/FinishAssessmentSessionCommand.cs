using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;

public sealed record FinishAssessmentSessionCommand(ParticipantId ModeratorId, string ModeratorName)
    : IRequest<FinishAssessmentSessionResult>, IWithModerator;