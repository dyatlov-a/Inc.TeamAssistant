using Inc.TeamAssistant.CheckIn.Primitives;

namespace Inc.TeamAssistant.CheckIn.Model.Commands.AddLocationToMap;

public sealed record AddLocationToMapResult(MapId MapId, bool IsCreated);