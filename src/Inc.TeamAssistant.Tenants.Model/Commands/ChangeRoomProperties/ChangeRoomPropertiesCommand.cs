using MediatR;

namespace Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;

public sealed record ChangeRoomPropertiesCommand(
    Guid RoomId,
    bool? IsFacilitator,
    Guid? RetroTemplateId,
    Guid? SurveyTemplateId,
    TimeSpan? TimerDuration,
    int? VoteCount,
    int? VoteByItemCount,
    string? RetroType)
    : IRequest
{
    public static ChangeRoomPropertiesCommand ChangeFacilitator(Guid roomId)
    {
        return new ChangeRoomPropertiesCommand(
            roomId,
            IsFacilitator: true,
            RetroTemplateId: null,
            SurveyTemplateId: null,
            TimerDuration: null,
            VoteCount: null,
            VoteByItemCount: null,
            RetroType: string.Empty);
    }
    
    public static ChangeRoomPropertiesCommand ChangeProperties(
        Guid roomId,
        Guid retroTemplateId,
        Guid surveyTemplateId,
        TimeSpan timerDuration,
        int voteCount,
        int voteByItemCount,
        string retroType)
    {
        return new ChangeRoomPropertiesCommand(
            roomId,
            IsFacilitator: false,
            retroTemplateId,
            surveyTemplateId,
            timerDuration,
            voteCount,
            voteByItemCount,
            retroType);
    }
}