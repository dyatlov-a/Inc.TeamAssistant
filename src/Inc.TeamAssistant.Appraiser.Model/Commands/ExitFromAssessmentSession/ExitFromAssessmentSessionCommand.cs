using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;

public sealed record ExitFromAssessmentSessionCommand(
        long TargetChatId,
        ParticipantId AppraiserId,
        string AppraiserName)
    : IRequest<CommandResult>, IWithAppraiser;