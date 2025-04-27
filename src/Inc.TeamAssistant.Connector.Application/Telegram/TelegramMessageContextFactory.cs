using System.Text;
using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Parsers;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Inc.TeamAssistant.Connector.Application.Telegram;

internal sealed class TelegramMessageContextFactory
{
    private readonly IPersonRepository _personRepository;
    private readonly IClientLanguageRepository _languageRepository;
    private readonly MessageParser _messageParser;

    public TelegramMessageContextFactory(
        IPersonRepository personRepository,
        IClientLanguageRepository languageRepository,
        MessageParser messageParser)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
        _messageParser = messageParser ?? throw new ArgumentNullException(nameof(messageParser));
    }
    
    public async Task<MessageContext?> Create(Bot bot, Update update, CancellationToken token)
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
        
        var parsedText = await _messageParser.Parse(TelegramMessageAdapter.Create(bot.Name, editedMessage), token);
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
            replyToMessageId: editedMessage.ReplyToMessage?.MessageId,
            token);
    }

    private async Task<MessageContext?> CreateFromPollAnswer(Bot bot, PollAnswer pollAnswer, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(pollAnswer);
        
        var text = pollAnswer.OptionIds.Aggregate(
            new StringBuilder(string.Format(CommandList.AddPollAnswer, pollAnswer.PollId)),
            (sb, p) => sb.Append(GlobalResources.Settings.OptionParameterName).Append(p),
            sb => sb.ToString());
            
        return await Create(
            bot,
            messageId: 0,
            chatId: 0,
            chatName: null,
            pollAnswer.User,
            text,
            targetPersonId: null,
            location: null,
            replyToMessageId: null,
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
            replyToMessageId: null,
            token);
    }

    private async Task<MessageContext?> CreateFromMessage(
        Bot bot,
        Message message,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(message);
        
        var parsedText = await _messageParser.Parse(TelegramMessageAdapter.Create(bot.Name, message), token);

        return await Create(
            bot,
            message.MessageId,
            message.Chat.Id,
            message.Chat.Title,
            message.From!,
            parsedText.Text,
            parsedText.TargetPersonId,
            message.Location,
            message.ReplyToMessage?.MessageId,
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
        int? replyToMessageId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(text);

        var person = await EnsurePerson(user, token);
        var language = await EnsureLanguage(bot.Id, user, token);
        var teams = GetTeams(bot, person.Id, chatId);
            
        return MessageContext.Create(
            new ChatMessage(chatId, messageId, replyToMessageId),
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
            await _languageRepository.Upsert(botId, user.Id, user.LanguageCode, DateTimeOffset.UtcNow, token);
        
        var language = await _languageRepository.Get(botId, user.Id, token);
        return language;
    }

    private async Task<Person> EnsurePerson(User user, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        await _personRepository.Upsert(new Person(user.Id, user.FirstName, user.Username), token);
        
        var person = await _personRepository.Find(user.Id, token);
        return person!;
    }

    private IReadOnlyList<TeamContext> GetTeams(Bot bot, long personId, long chatId)
    {
        ArgumentNullException.ThrowIfNull(bot);
        
        var memberOfChats = bot.Teams
            .Where(t => MemberOfTeam(t) || t.ChatId == chatId || t.Owner.Id == personId)
            .Select(t => t.ChatId)
            .Distinct()
            .ToArray();
        var results = bot.Teams
            .Where(t => memberOfChats.Contains(t.ChatId))
            .Select(t => new TeamContext(t.Id, t.ChatId, t.Name, MemberOfTeam(t), OwnerOfTeam(t)))
            .ToArray();

        return results;
        
        bool MemberOfTeam(Team t) => t.Teammates.Any(tm => tm.Id == personId);
        bool OwnerOfTeam(Team t) => t.Owner.Id == personId || bot.OwnerId == personId;
    }
}