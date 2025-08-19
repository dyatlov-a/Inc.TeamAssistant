namespace Inc.TeamAssistant.Primitives.Features.Tenants;

public sealed record RoomId(string GroupName)
{
    public static RoomId CreateForRetro(Guid value) => new($"{value:N}_retro");
    
    public static RoomId CreateForSurvey(Guid value) => new($"{value:N}_survey");
}