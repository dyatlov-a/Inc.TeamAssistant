namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public sealed record RoomProperties(
    long? FacilitatorId,
    Guid RetroTemplateId,
    Guid SurveyTemplateId,
    TimeSpan TimerDuration,
    int VoteCount,
    int VoteByItemCount,
    string RetroType)
{
    public static RoomProperties Default { get; } = new(
        FacilitatorId: null,
        RetroTemplateId: Guid.Parse("41c7a7b9-044f-46aa-b94e-e3bb06aed70c"),
        SurveyTemplateId: Guid.Parse("6c9b2eef-b7ce-4e13-b866-1a0cd743c6b3"),
        TimerDuration: TimeSpan.FromMinutes(10),
        VoteCount: 5,
        VoteByItemCount: 2,
        RetroType: "Hidden");
}