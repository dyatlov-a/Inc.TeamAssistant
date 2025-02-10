using Inc.TeamAssistant.Constructor.Model.Commands.SetBotDetails;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class BotDetailsFormModel
{
    private readonly List<BotDetailsItemFormModel> _botDetails = new();
    public IReadOnlyCollection<BotDetailsItemFormModel> BotDetails => _botDetails;
    
    public BotDetailsFormModel Apply(GetBotDetailsResult botDetails)
    {
        ArgumentNullException.ThrowIfNull(botDetails);
        
        _botDetails.Clear();
        _botDetails.AddRange(botDetails.Items.Select(b => new BotDetailsItemFormModel
        {
            LanguageId = b.LanguageId,
            Name = b.Name,
            ShortDescription = b.ShortDescription,
            Description = b.Description
        }));

        return this;
    }

    public void Apply(IReadOnlyCollection<string> languageIds)
    {
        ArgumentNullException.ThrowIfNull(languageIds);
        
        var targetLanguages = _botDetails
            .Where(b => languageIds.Contains(b.LanguageId))
            .ToArray();
        
        _botDetails.Clear();
        _botDetails.AddRange(targetLanguages);
    }

    public BotDetailsItemFormModel AddLanguage(string languageId)
    {
        ArgumentNullException.ThrowIfNull(languageId);

        var item = new BotDetailsItemFormModel
        {
            LanguageId = languageId
        };
        
        _botDetails.Add(item);

        return item;
    }
    
    public SetBotDetailsCommand ToCommand(string token)
    {
        var botDetails = BotDetails
            .Select(d => new BotDetails(d.LanguageId, d.Name, d.ShortDescription, d.Description))
            .ToArray();
        
        return new SetBotDetailsCommand(token, botDetails);
    }
}