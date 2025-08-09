using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;
using Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

namespace Inc.TeamAssistant.WebUI.Features.RoomProperties;

public sealed class RoomPropertiesFormModel
{
    public Guid RetroTemplateId { get; private set; }
    public Guid SurveyTemplateId { get; private set; }
    public TimeSpan TimerDuration { get; private set; }
    public int VoteCount { get; private set; }
    public int VoteByItemCount { get; private set; }
    public string RetroType { get; private set; } = default!;
    
    public RoomPropertiesFormModel ChangeRetroTemplate(Guid value)
    {
        RetroTemplateId = value;
        
        return this;
    }
    
    public RoomPropertiesFormModel ChangeSurveyTemplate(Guid value)
    {
        SurveyTemplateId = value;
        
        return this;
    }
    
    public RoomPropertiesFormModel ChangeTimerDuration(TimeSpan value)
    {
        TimerDuration = value;
        
        return this;
    }
    
    public RoomPropertiesFormModel ChangeVoteCount(int value)
    {
        VoteCount = value;

        if (VoteByItemCount > value)
            ChangeVoteByItemCount(value);
        
        return this;
    }
    
    public RoomPropertiesFormModel ChangeVoteByItemCount(int value)
    {
        VoteByItemCount = value;
        
        return this;
    }
    
    public RoomPropertiesFormModel ChangeRetroType(string value)
    {
        RetroType = value;
        
        return this;
    }
    
    public RoomPropertiesFormModel Apply(GetRoomPropertiesResult model)
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