using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AcceptEstimate;

public sealed record AcceptEstimateCommand(MessageContext MessageContext, Guid StoryId)
    : IEndDialogCommand;