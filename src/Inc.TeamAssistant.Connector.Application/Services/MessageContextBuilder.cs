using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class MessageContextBuilder
{
    private readonly PersonService _personService;
    private readonly IPersonRepository _personRepository;

    public MessageContextBuilder(PersonService personService, IPersonRepository personRepository)
    {
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
    }

    public async Task<MessageContext?> Build(Bot bot, Update update, CancellationToken token)
    {
        if (bot is null)
            throw new ArgumentNullException(nameof(bot));
        if (update is null)
            throw new ArgumentNullException(nameof(update));
        
        if (update.Message?.From?.IsBot == true || update.CallbackQuery?.From.IsBot == true)
            return null;

        var messageId = update.Message?.MessageId ?? update.CallbackQuery?.Message?.MessageId;
        if (!messageId.HasValue)
            return null;

        var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;
        if (!chatId.HasValue)
            return null;

        var user = update.Message?.From ?? update.CallbackQuery?.From;
        if (user is null)
            return null;
        
        var message = await ParseText(
            bot.Name,
            update.Message,
            update.CallbackQuery?.Data,
            update.Message?.Location,
            token);

        return message.HasValue
            ? await Create(
                bot,
                messageId.Value,
                chatId.Value,
                user,
                message.Value.Text,
                message.Value.TargetPersonId,
                update.Message?.Location,
                token)
            : null;
    }

    private async Task<MessageContext?> Create(
        Bot bot,
        int messageId,
        long chatId,
        User user,
        string text,
        long? targetPersonId,
        Location? location,
        CancellationToken token)
    {
        if (bot is null)
            throw new ArgumentNullException(nameof(bot));
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        var person = await _personService.EnsurePerson(user, token);
        var teams = GetTeams(bot, person.Id, chatId);
            
        return new(
            messageId,
            bot.Id,
            bot.Name,
            teams,
            text,
            chatId,
            user.Id,
            user.FirstName,
            user.Username,
            person.LanguageId,
            location is not null ? new (location.Longitude, location.Latitude) : null,
            targetPersonId);
    }
    
    private async Task<(string Text, long? TargetPersonId)?> ParseText(
        string botName,
        Message? message,
        string? data,
        Location? location,
        CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        if (!string.IsNullOrWhiteSpace(message?.Text))
        {
            var text = message.Text.Replace($"@{botName} ", string.Empty);
            var attachedPerson = await GetPersonFromEntities(message, token) ?? await GetPersonFromText(text, token);
            
            if (attachedPerson.HasValue)
            {
                var cleanText = text.Replace(attachedPerson.Value.Marker, string.Empty);
                
                return string.IsNullOrWhiteSpace(cleanText)
                    ? null
                    : (cleanText, attachedPerson.Value.Id);
            }

            return (text, null);
        }

        return string.IsNullOrWhiteSpace(data)
            ? location is not null
                ? ("/location", null)
                : null
            : (data, null);
    }

    private IReadOnlyList<TeamContext> GetTeams(Bot bot, long personId, long chatId)
    {
        if (bot is null)
            throw new ArgumentNullException(nameof(bot));

        bool MemberOfTeam(Team t) => t.Teammates.Any(tm => tm.Id == personId);
        
        var memberOfChats = bot.Teams
            .Where(t => MemberOfTeam(t) || t.ChatId == chatId || t.OwnerId == personId)
            .Select(t => t.ChatId)
            .Distinct()
            .ToArray();
        var results = bot.Teams
            .Where(t => memberOfChats.Contains(t.ChatId))
            .Select(t => new TeamContext(t.Id, t.ChatId, t.Name, MemberOfTeam(t)))
            .ToArray();

        return results;
    }

    private async Task<(long Id, string Marker)?> GetPersonFromEntities(Message message, CancellationToken token)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        var personId = message.Entities
            ?.LastOrDefault(e => e is { Type: MessageEntityType.TextMention, User: not null })
            ?.User!.Id;

        if (personId.HasValue)
        {
            var person = await _personRepository.Find(personId.Value, token);
            
            return person is not null
                ? (person.Id, person.Name)
                : null;
        }

        return null;
    }

    private async Task<(long Id, string Marker)?> GetPersonFromText(string text, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));
        
        const char usernameMarker = '@';
        var username = text.Split(usernameMarker).LastOrDefault()?.Trim();

        if (string.IsNullOrWhiteSpace(username))
            return null;
        
        var person = await _personRepository.Find(username, token);
        
        return person is not null
            ? (person.Id, $"{usernameMarker}{username}")
            : null;
    }
}