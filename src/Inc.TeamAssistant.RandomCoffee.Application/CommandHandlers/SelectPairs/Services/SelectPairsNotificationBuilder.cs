using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Domain;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.SelectPairs.Services;

internal sealed class SelectPairsNotificationBuilder
{
    private readonly ITeamAccessor _teamAccessor;
    private readonly IMessageBuilder _messageBuilder;

    public SelectPairsNotificationBuilder(ITeamAccessor teamAccessor, IMessageBuilder messageBuilder)
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
        var builder = NotificationBuilder.Create()
            .Add(sb => sb.AppendLine(_messageBuilder.Build(Messages.RandomCoffee_SelectedPairs, languageId)));
            
        foreach (var pair in randomCoffeeHistory.Pairs)
        {
            var firstPerson = await _teamAccessor.FindPerson(pair.FirstId, token);
            var secondPerson = await _teamAccessor.FindPerson(pair.SecondId, token);

            if (firstPerson is not null && secondPerson is not null)
            {
                var firstLanguageId = await _teamAccessor.GetClientLanguage(botId, firstPerson.Id, token);
                var secondLanguageId = await _teamAccessor.GetClientLanguage(botId, secondPerson.Id, token);

                builder
                    .Add(sb => firstPerson
                        .AddTo(sb, (p, o) => attachPersons += n => n.AttachPerson(p, firstLanguageId, o))
                        .AddSeparator(" - "))
                    .Add(sb => secondPerson
                        .AddTo(sb, (p, o) => attachPersons += n => n.AttachPerson(p, secondLanguageId, o))
                        .AppendLine());
            }
        }

        if (randomCoffeeHistory.ExcludedPersonId.HasValue)
        {
            var excludedPerson = await _teamAccessor.FindPerson(randomCoffeeHistory.ExcludedPersonId.Value, token);

            if (excludedPerson is not null)
            {
                var excludedLanguageId = await _teamAccessor.GetClientLanguage(botId, excludedPerson.Id, token);

                builder
                    .Add(sb => sb
                        .AppendLine()
                        .Append(_messageBuilder.Build(Messages.RandomCoffee_NotSelectedPair, languageId))
                        .AddSeparator())
                    .Add(sb => excludedPerson
                        .AddTo(sb, (p, o) => attachPersons += n => n.AttachPerson(p, excludedLanguageId, o))
                        .AppendLine());
            }
        }

        var notification = attachPersons(builder
            .Add(sb => sb
                .AppendLine()
                .AppendLine(_messageBuilder.Build(Messages.RandomCoffee_MeetingDescription, languageId)))
            .Build(m => NotificationMessage.Create(chatId, m)));
        
        return notification;
    }
}