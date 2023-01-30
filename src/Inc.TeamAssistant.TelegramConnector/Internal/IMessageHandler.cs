using Telegram.Bot;
using Telegram.Bot.Types;

namespace Inc.TeamAssistant.TelegramConnector.Internal;

internal interface IMessageHandler
{
    Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken);

    Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken);
}