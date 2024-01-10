using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;

public sealed record ReVoteEstimateCommand(MessageContext MessageContext, Guid StoryId)
    : IRequest<CommandResult>;