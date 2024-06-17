using Inc.TeamAssistant.Connector.Application.Extensions;
using Inc.TeamAssistant.Primitives.Commands;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

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
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        await _dialogContinuation.End(
            command.MessageContext.Bot.Id,
            command.MessageContext.TargetChat,
            command.MessageContext.ChatMessage,
            async (ms, t) =>
            {
                if (command.MessageContext.Shared && ms.Any())
                {
                    var client = await _provider.Get(command.MessageContext.Bot.Id, t);
                    await client.TryDeleteMessages(command.MessageContext.Bot.Id, ms, _logger, token);
                }
            },
            token);
    }
}