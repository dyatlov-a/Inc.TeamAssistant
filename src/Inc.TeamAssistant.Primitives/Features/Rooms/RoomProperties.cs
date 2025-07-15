namespace Inc.TeamAssistant.Primitives.Features.Rooms;

public sealed record RoomProperties(
    long? FacilitatorId,
    Guid? RetroTemplateId,
    Guid? SurveyTemplateId,
    TimeSpan? TimerDuration,
    int? VoteCount,
    int? VoteByItemCount,
    string? RetroType)
{
    private static readonly Guid DefaultRetroTemplateId = Guid.Parse("41c7a7b9-044f-46aa-b94e-e3bb06aed70c");
    
    public string RequiredRetroType()
    {
        const string defaultRetroType = "Hidden";
        
        return string.IsNullOrWhiteSpace(RetroType) ? defaultRetroType : RetroType;
    }

    public Guid RequiredRetroTemplateId() => RetroTemplateId ?? DefaultRetroTemplateId;
}