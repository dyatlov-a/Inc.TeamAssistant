using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed class BotFormModel
{
    public Guid? BotId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public static BotFormModel Create(GetBotResult bot)
    {
        ArgumentNullException.ThrowIfNull(bot);
        
        return new BotFormModel
        {
            BotId = bot.Id,
            UserName = bot.UserName,
            Token = bot.Token
        };
    }
}