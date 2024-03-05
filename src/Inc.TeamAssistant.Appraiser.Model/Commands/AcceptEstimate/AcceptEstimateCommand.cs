using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;

public sealed record AcceptEstimateCommand(MessageContext MessageContext, Guid StoryId)
    : IRequest<CommandResult>;