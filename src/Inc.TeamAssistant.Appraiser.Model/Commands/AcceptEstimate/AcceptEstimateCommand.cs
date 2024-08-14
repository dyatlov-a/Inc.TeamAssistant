using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;

public sealed record AcceptEstimateCommand(MessageContext MessageContext, Guid StoryId, string Value)
    : IDialogCommand;