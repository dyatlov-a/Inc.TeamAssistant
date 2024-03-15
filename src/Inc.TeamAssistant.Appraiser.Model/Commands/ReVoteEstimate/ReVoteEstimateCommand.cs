using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;

public sealed record ReVoteEstimateCommand(MessageContext MessageContext, Guid StoryId)
    : IEndDialogCommand;