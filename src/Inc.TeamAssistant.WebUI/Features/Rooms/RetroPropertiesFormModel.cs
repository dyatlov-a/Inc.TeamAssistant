using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
using Inc.TeamAssistant.Tenants.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Rooms;

public sealed class RetroPropertiesFormModel
{
    public Guid RetroTemplateId { get; private set; }
    public Guid SurveyTemplateId { get; private set; }
    public TimeSpan TimerDuration { get; private set; }
    public int VoteCount { get; private set; }
    public int VoteByItemCount { get; private set; }
    public string RetroType { get; private set; } = default!;
    
    public RetroPropertiesFormModel ChangeRetroTemplate(Guid value)
    {
        RetroTemplateId = value;
        
        return this;
    }
    
    public RetroPropertiesFormModel ChangeSurveyTemplate(Guid value)
    {
        SurveyTemplateId = value;
        
        return this;
    }
    
    public RetroPropertiesFormModel ChangeTimerDuration(TimeSpan value)
    {
        TimerDuration = value;
        
        return this;
    }
    
    public RetroPropertiesFormModel ChangeVoteCount(int value)
    {
        VoteCount = value;

        if (VoteByItemCount > value)
            ChangeVoteByItemCount(value);
        
        return this;
    }
    
    public RetroPropertiesFormModel ChangeVoteByItemCount(int value)
    {
        VoteByItemCount = value;
        
        return this;
    }
    
    public RetroPropertiesFormModel ChangeRetroType(string value)
    {
        RetroType = value;
        
        return this;
    }
    
    public RetroPropertiesFormModel Apply(RoomPropertiesDto model)
    {
        ArgumentNullException.ThrowIfNull(model);
        
        RetroTemplateId = model.RetroTemplateId;
        SurveyTemplateId = model.SurveyTemplateId;
        RetroType = model.RetroType;
        TimerDuration = model.TimerDuration;
        VoteCount = model.VoteCount;
        VoteByItemCount = model.VoteByItemCount;
        
        return this;
    }
    
    public ChangeRoomPropertiesCommand ToCommand(Guid roomId)
    {
        return ChangeRoomPropertiesCommand.ChangeProperties(
            roomId,
            RetroTemplateId,
            SurveyTemplateId,
            TimerDuration,
            VoteCount,
            VoteByItemCount,
            RetroType);
    }
}