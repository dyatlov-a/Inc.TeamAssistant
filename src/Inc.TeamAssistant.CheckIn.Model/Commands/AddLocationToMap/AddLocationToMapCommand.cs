using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;

public sealed record AddLocationToMapCommand(MessageContext MessageContext)
    : IEndDialogCommand;