using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class RetroPropertiesFormModel
{
    public Guid TemplateId { get; private set; }
    public TimeSpan TimerDuration { get; private set; }
    public int VoteCount { get; private set; }
    public int VoteByItemCount { get; private set; }
    public string RetroType { get; private set; } = default!;
    
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
    
    public RetroPropertiesFormModel Apply(RetroPropertiesDto model)
    {
        ArgumentNullException.ThrowIfNull(model);
        
        TemplateId = model.TemplateId;
        RetroType = model.RetroType;
        TimerDuration = model.TimerDuration;
        VoteCount = model.VoteCount;
        VoteByItemCount = model.VoteByItemCount;
        
        return this;
    }
    
    public ChangeRetroPropertiesCommand ToCommand(Guid roomId)
    {
        return ChangeRetroPropertiesCommand.ChangeProperties(
            roomId,
            TemplateId,
            TimerDuration,
            VoteCount,
            VoteByItemCount,
            RetroType);
    }
}