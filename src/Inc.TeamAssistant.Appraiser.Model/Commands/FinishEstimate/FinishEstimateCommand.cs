using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.FinishEstimate;

public sealed record FinishEstimateCommand(MessageContext MessageContext, Guid StoryId)
    : IEndDialogCommand;