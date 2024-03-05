using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;

public sealed record AddLocationToMapCommand(MessageContext MessageContext)
    : IRequest<CommandResult>;