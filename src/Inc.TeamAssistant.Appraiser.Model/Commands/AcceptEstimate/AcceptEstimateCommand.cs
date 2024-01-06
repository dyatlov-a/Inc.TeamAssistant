using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;

public sealed record AcceptEstimateCommand(long ModeratorId, string ModeratorName)
    : IRequest<CommandResult>, IWithModerator;