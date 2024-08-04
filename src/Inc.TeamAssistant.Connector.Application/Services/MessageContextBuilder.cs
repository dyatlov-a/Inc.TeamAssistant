using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class MessageContextBuilder
{
    private readonly IPersonRepository _personRepository;
    private readonly IClientLanguageRepository _clientLanguageRepository;

    public MessageContextBuilder(
        IPersonRepository personRepository,
        IClientLanguageRepository clientLanguageRepository)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _clientLanguageRepository = clientLanguageRepository ?? throw new ArgumentNullException(nameof(clientLanguageRepository));
    }
    
    public async Task<MessageContext?> Build(Bot bot, Update update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(update);

        if (update.Message?.From?.IsBot == true ||
            update.CallbackQuery?.From.IsBot == true ||
            update.PollAnswer?.User.IsBot == true)
            return null;

        return update.Type switch
        {
            UpdateType.Message => await CreateFromMessage(bot, update.Message!, token),
            UpdateType.CallbackQuery => await CreateFromCallbackQuery(bot, update.CallbackQuery!, token),
            UpdateType.PollAnswer => await CreateFromPollAnswer(bot, update.PollAnswer!, token),
            UpdateType.EditedMessage => await CreateFromEdited(bot, update.EditedMessage!, token),
            _ => null
        };
    }

    private async Task<MessageContext?> CreateFromEdited(Bot bot, Message editedMessage, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(editedMessage);
        
        var parsedText = await ParseText(bot.Name, editedMessage, token);
        var text = string.Format(CommandList.EditDraft, parsedText.Text);
        
        return await Create(
            bot,
            editedMessage.MessageId,
            editedMessage.Chat.Id,
            editedMessage.Chat.Title,
            editedMessage.From!,
            text,
            targetPersonId: parsedText.TargetPersonId,
            location: null,
            token);
    }

    private async Task<MessageContext?> CreateFromPollAnswer(Bot bot, PollAnswer pollAnswer, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(pollAnswer);
        
        var parameters = string.Join("&option=", pollAnswer.OptionIds);
        var text = string.Format(CommandList.AddPollAnswer, pollAnswer.PollId, parameters);
            
        return await Create(
            bot,
            messageId: 0,
            chatId: 0,
            chatName: null,
            pollAnswer.User,
            text,
            targetPersonId: null,
            location: null,
            token);
    }

    private async Task<MessageContext?> CreateFromCallbackQuery(
        Bot bot,
        CallbackQuery callbackQuery,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(callbackQuery);
        
        return await Create(
            bot,
            callbackQuery.Message!.MessageId,
            callbackQuery.Message.Chat.Id,
            callbackQuery.Message.Chat.Title,
            callbackQuery.From,
            callbackQuery.Data!,
            targetPersonId: null,
            location: null,
            token);
    }

    private async Task<MessageContext?> CreateFromMessage(
        Bot bot,
        Message message,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(message);
        
        var parsedText = await ParseText(bot.Name, message, token);

        return await Create(
            bot,
            message.MessageId,
            message.Chat.Id,
            message.Chat.Title,
            message.From!,
            parsedText.Text,
            targetPersonId: parsedText.TargetPersonId,
            location: message.Location,
            token);
    }

    private async Task<MessageContext?> Create(
        Bot bot,
        int messageId,
        long chatId,
        string? chatName,
        User user,
        string text,
        long? targetPersonId,
        Location? location,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(text);

        var person = await EnsurePerson(user, token);
        var language = await EnsureLanguage(bot.Id, user, token);
        var teams = GetTeams(bot, person.Id, chatId);
            
        return new(
            new ChatMessage(chatId, messageId),
            new BotContext(bot.Id, bot.Name, bot.Properties),
            teams,
            text,
            person,
            language,
            location is not null ? new (location.Longitude, location.Latitude) : null,
            targetPersonId,
            chatName);
    }
    
    private async Task<LanguageId> EnsureLanguage(Guid botId, User user, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        if (!string.IsNullOrWhiteSpace(user.LanguageCode))
            await _clientLanguageRepository.Upsert(botId, user.Id, user.LanguageCode, DateTimeOffset.UtcNow, token);
        
        var language = await _clientLanguageRepository.Get(botId, user.Id, token);
        return language;
    }

    private async Task<Person> EnsurePerson(User user, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        await _personRepository.Upsert(new Person(user.Id, user.FirstName, user.Username), token);
        
        var person = await _personRepository.Find(user.Id, token);
        return person!;
    }
    
    private async Task<(string Text, long? TargetPersonId)> ParseText(
        string botName,
        Message message,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(message);
        
        if (string.IsNullOrWhiteSpace(botName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botName));

        var input = message.Text ?? String.Empty;
        var text = input.Replace($"@{botName} ", string.Empty);
        var attachedPerson = await GetPersonFromEntities(message, token) ?? await GetPersonFromText(text, token);

        return attachedPerson.HasValue
            ? (text.Replace(attachedPerson.Value.Marker, string.Empty), attachedPerson.Value.Id)
            : (text, null);
    }

    private IReadOnlyList<TeamContext> GetTeams(Bot bot, long personId, long chatId)
    {
        ArgumentNullException.ThrowIfNull(bot);

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
        ArgumentNullException.ThrowIfNull(message);

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
            return null;
        
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