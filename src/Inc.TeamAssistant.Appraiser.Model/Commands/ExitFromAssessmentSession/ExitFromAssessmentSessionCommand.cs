using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;

public sealed record ExitFromAssessmentSessionCommand(
        long TargetChatId,
        long AppraiserId,
        string AppraiserName)
    : IRequest<CommandResult>, IWithAppraiser;