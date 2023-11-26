using Inc.TeamAssistant.Appraiser.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;

public sealed record ReVoteEstimateCommand(long ModeratorId, string ModeratorName)
    : IRequest<CommandResult>, IWithModerator;