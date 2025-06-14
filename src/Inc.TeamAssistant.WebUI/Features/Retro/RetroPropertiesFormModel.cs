using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class RetroPropertiesFormModel
{
    public Guid TemplateId { get; private set; }
    public TimeSpan TimerDuration { get; private set; }
    public int VoteCount { get; private set; }
    
    public RetroPropertiesFormModel ChangeTemplate(Guid value)
    {
        TemplateId = value;
        
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
        
        return this;
    }
    
    public RetroPropertiesFormModel Apply(RetroPropertiesDto model)
    {
        ArgumentNullException.ThrowIfNull(model);
        
        TemplateId = model.TemplateId;
        TimerDuration = model.TimerDuration;
        VoteCount = model.VoteCount;
        
        return this;
    }
    
    public ChangeRetroPropertiesCommand ToCommand(Guid roomId)
    {
        return ChangeRetroPropertiesCommand.ChangeProperties(roomId, TemplateId, TimerDuration, VoteCount);
    }
}