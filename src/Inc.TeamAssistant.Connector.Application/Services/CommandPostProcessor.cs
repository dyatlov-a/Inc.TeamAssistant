using Inc.TeamAssistant.Primitives;
using MediatR.Pipeline;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandPostProcessor<TCommand, TResult> : IRequestPostProcessor<TCommand, TResult>
    where TCommand : IEndDialogCommand
{
    private readonly DialogContinuation _dialogContinuation;
    private readonly TelegramBotClientProvider _provider;

    public CommandPostProcessor(DialogContinuation dialogContinuation, TelegramBotClientProvider provider)
    {
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task Process(TCommand command, TResult result, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        await _dialogContinuation.End(
            command.MessageContext.PersonId,
            new ChatMessage(command.MessageContext.ChatId, command.MessageContext.MessageId),
            async (ms, t) =>
            {
                if (command.MessageContext.Shared && ms.Any())
                {
                    var client = await _provider.Get(command.MessageContext.BotId, t);
                    
                    foreach (var m in ms)
                        await client.DeleteMessageAsync(m.ChatId, m.MessageId, cancellationToken: t);
                }
            },
            token);
    }
}