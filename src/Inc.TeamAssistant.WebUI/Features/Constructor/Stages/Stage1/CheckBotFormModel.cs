using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage1;

public sealed class CheckBotFormModel
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public bool HasAccess { get; set; }

    public CheckBotFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);
        
        UserName = stagesState.UserName;
        Token = stagesState.Token;

        return this;
    }

    public CheckBotFormModel Apply(GetBotUserNameResult checkResult)
    {
        ArgumentNullException.ThrowIfNull(checkResult);
        
        UserName = checkResult.UserName;
        HasAccess = checkResult.HasAccess;

        return this;
    }

    public CheckBotFormModel Clear()
    {
        UserName = string.Empty;
        HasAccess = false;

        return this;
    }
}