namespace Inc.TeamAssistant.Retro.Model.Common;

public sealed record RetroSessionDto(
    Guid Id,
    Guid TeamId,
    DateTimeOffset Created,
    string State,
    long FacilitatorId);