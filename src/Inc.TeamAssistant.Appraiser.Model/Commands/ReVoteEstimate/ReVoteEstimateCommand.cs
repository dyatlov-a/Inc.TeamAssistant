using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;

public sealed record ReVoteEstimateCommand(MessageContext MessageContext, Guid StoryId)
    : IEndDialogCommand;