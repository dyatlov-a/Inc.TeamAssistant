using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class BotDetailsFormModel
{
    public IReadOnlyCollection<BotDetailsItemFormModel> BotDetails { get; set; } = Array.Empty<BotDetailsItemFormModel>();
    
    public BotDetailsFormModel Apply(GetBotDetailsResult botDetails)
    {
        ArgumentNullException.ThrowIfNull(botDetails);
        
        BotDetails = botDetails.Items.Select(b => new BotDetailsItemFormModel
        {
            LanguageId = b.LanguageId,
            Name = b.Name,
            ShortDescription = b.ShortDescription,
            Description = b.Description
        }).ToArray();

        return this;
    }
    
    public SetBotDetailsCommand ToCommand(string token)
    {
        var botDetails = BotDetails
            .Select(d => new BotDetails(d.LanguageId, d.Name, d.ShortDescription, d.Description))
            .ToArray();
        
        return new SetBotDetailsCommand(token, botDetails);
    }
}