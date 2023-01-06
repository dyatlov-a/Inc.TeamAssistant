using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;

public sealed record ExitFromAssessmentSessionCommand(ParticipantId AppraiserId, string AppraiserName)
    : IRequest<ExitFromAssessmentSessionResult>, IWithAppraiser;