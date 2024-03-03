using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class MessageContextBuilder
{
    private readonly PersonService _personService;

    public MessageContextBuilder(PersonService personService)
    {
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
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

        var text = ExtractText(update.Message?.Text, update.CallbackQuery?.Data, update.Message?.Location);
        if (string.IsNullOrWhiteSpace(text))
            return null;

        return await Create(bot, messageId.Value, chatId.Value, user, text, update.Message?.Location, token);
    }

    private async Task<MessageContext?> Create(
        Bot bot,
        int messageId,
        long chatId,
        User user,
        string text,
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
            location is not null ? new (location.Longitude, location.Latitude) : null);
    }
    
    private string ExtractText(string? text, string? data, Location? location)
    {
        return string.IsNullOrWhiteSpace(text)
            ? string.IsNullOrWhiteSpace(data)
                ? location is not null
                    ? "/location"
                    : string.Empty
                : data
            : text;
    }

    private IReadOnlyList<TeamContext> GetTeams(Bot bot, long personId, long chatId)
    {
        if (bot is null)
            throw new ArgumentNullException(nameof(bot));
        
        return bot.Teams
            .Where(t => t.Teammates.Any(tm => tm.Id == personId) || t.ChatId == chatId)
            .Select(t => new TeamContext(
                t.Id,
                t.ChatId,
                t.Name,
                t.Teammates.Any(tm => tm.Id == personId)))
            .ToArray();
    }
}