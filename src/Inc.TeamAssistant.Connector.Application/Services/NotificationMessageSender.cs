using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Application.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class NotificationMessageSender : INotificationMessageSender
{
    private readonly Dictionary<Guid, TelegramBotClient> _clientsByBots = new();
    private readonly Dictionary<Guid, Guid> _botsByTeams = new();
    
    private readonly IBotRepository _botRepository;

    public NotificationMessageSender(IBotRepository botRepository)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task Send(Guid teamId, NotificationMessage notification, CancellationToken token)
    {
        if (notification is null)
            throw new ArgumentNullException(nameof(notification));

        if (!notification.TargetChatId.HasValue)
            throw new ApplicationException("NotificationMessage target is not supported.");

        var client = await EnsureClient(teamId, token);
        
        await client.SendTextMessageAsync(
            notification.TargetChatId.Value,
            notification.Text,
            replyMarkup: notification.ToReplyMarkup(),
            cancellationToken: token);
    }

    private async Task<TelegramBotClient> EnsureClient(Guid teamId, CancellationToken token)
    {
        if (_botsByTeams.TryGetValue(teamId, out var botId) && _clientsByBots.TryGetValue(botId, out var cachedClient))
            return cachedClient;

        var bot = await _botRepository.FindByTeam(teamId, token);
        if (bot is null)
            throw new TeamAssistantUserException(Messages.Connector_BotNotFound, teamId);

        var client = new TelegramBotClient(bot.Token);
        
        _botsByTeams[teamId] = bot.Id;
        _clientsByBots[bot.Id] = client;

        return client;
    }
}