using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;

public sealed record AddLocationToMapCommand(MessageContext MessageContext)
    : IEndDialogCommand;