using System.Text;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs.Services;

internal sealed class NotificationsBuilder
{
    private readonly ITeamAccessor _teamAccessor;
    private readonly IMessageBuilder _messageBuilder;

    public NotificationsBuilder(ITeamAccessor teamAccessor, IMessageBuilder messageBuilder)
    {
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    public async Task<NotificationMessage> Build(
        long chatId,
        Guid botId,
        LanguageId languageId,
        RandomCoffeeHistory randomCoffeeHistory,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(languageId);
        ArgumentNullException.ThrowIfNull(randomCoffeeHistory);

        Func<NotificationMessage, NotificationMessage> attachPersons = n => n;
        var builder = new StringBuilder();
        builder.AppendLine(await _messageBuilder.Build(Messages.RandomCoffee_SelectedPairs, languageId));
            
        foreach (var pair in randomCoffeeHistory.Pairs)
        {
            var firstPerson = await _teamAccessor.FindPerson(pair.FirstId, token);
            var secondPerson = await _teamAccessor.FindPerson(pair.SecondId, token);

            if (firstPerson is not null && secondPerson is not null)
            {
                var firstLanguageId = await _teamAccessor.GetClientLanguage(botId, firstPerson.Id, token);
                var secondLanguageId = await _teamAccessor.GetClientLanguage(botId, secondPerson.Id, token);
                
                firstPerson.Append(builder, (p, o) => attachPersons += n => n.AttachPerson(p, firstLanguageId, o));
                builder.Append(" - ");
                secondPerson.Append(builder, (p, o) => attachPersons += n => n.AttachPerson(p, secondLanguageId, o));
                builder.AppendLine();
            }
        }

        if (randomCoffeeHistory.ExcludedPersonId.HasValue)
        {
            var excludedPerson = await _teamAccessor.FindPerson(randomCoffeeHistory.ExcludedPersonId.Value, token);

            if (excludedPerson is not null)
            {
                var excludedLanguageId = await _teamAccessor.GetClientLanguage(botId, excludedPerson.Id, token);
                
                builder.AppendLine();
                builder.Append(await _messageBuilder.Build(Messages.RandomCoffee_NotSelectedPair, languageId));
                builder.Append(' ');
                excludedPerson.Append(builder, (p, o) => attachPersons += n => n.AttachPerson(p, excludedLanguageId, o));
                builder.AppendLine();
            }
        }

        builder.AppendLine();
        builder.AppendLine(await _messageBuilder.Build(Messages.RandomCoffee_MeetingDescription, languageId));
        
        return attachPersons(NotificationMessage.Create(chatId, builder.ToString()));
    }
}