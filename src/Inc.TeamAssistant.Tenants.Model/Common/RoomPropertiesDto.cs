namespace Inc.TeamAssistant.Tenants.Model.Common;

public sealed record RoomPropertiesDto(
    long? FacilitatorId,
    Guid RetroTemplateId,
    Guid SurveyTemplateId,
    TimeSpan TimerDuration,
    int VoteCount,
    int VoteByItemCount,
    string RetroType);