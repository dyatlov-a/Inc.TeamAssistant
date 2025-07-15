using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;

public sealed record ChangeRetroPropertiesCommand(
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
    public static ChangeRetroPropertiesCommand ChangeFacilitator(Guid roomId)
    {
        return new ChangeRetroPropertiesCommand(
            roomId,
            IsFacilitator: true,
            RetroTemplateId: null,
            SurveyTemplateId: null,
            TimerDuration: null,
            VoteCount: null,
            VoteByItemCount: null,
            RetroType: string.Empty);
    }
    
    public static ChangeRetroPropertiesCommand ChangeProperties(
        Guid roomId,
        Guid retroTemplateId,
        Guid surveyTemplateId,
        TimeSpan timerDuration,
        int voteCount,
        int voteByItemCount,
        string retroType)
    {
        return new ChangeRetroPropertiesCommand(
            roomId,
            IsFacilitator: true,
            retroTemplateId,
            surveyTemplateId,
            timerDuration,
            voteCount,
            voteByItemCount,
            retroType);
    }
}