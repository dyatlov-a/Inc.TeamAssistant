using Inc.TeamAssistant.Connector.Application.Telegram;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandPostProcessor<TCommand, TResult> : IRequestPostProcessor<TCommand, TResult>
    where TCommand : IEndDialogCommand
{
    private readonly DialogContinuation _dialogContinuation;
    private readonly TelegramBotClientProvider _provider;
    private readonly ILogger<CommandPostProcessor<TCommand, TResult>> _logger;

    public CommandPostProcessor(
        DialogContinuation dialogContinuation,
        TelegramBotClientProvider provider,
        ILogger<CommandPostProcessor<TCommand, TResult>> logger)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Process(TCommand command, TResult result, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var client = await _provider.Get(command.MessageContext.Bot.Id, token);
        var messages = _dialogContinuation.End(
            command.MessageContext.Bot.Id,
            command.MessageContext.TargetChat,
            command.SaveEndOfDialog ? null : command.MessageContext.ChatMessage);

        if (messages.Any())
            await TryDeleteMessages(client, messages, token);
    }
    
    private async Task TryDeleteMessages(
        ITelegramBotClient client,
        IReadOnlyCollection<ChatMessage> messages,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(messages);
        
        try
        {
            foreach (var message in messages)
                await client.DeleteMessageAsync(message.ChatId, message.MessageId, cancellationToken: token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bot has not rights for delete messages");
        }
    }
}