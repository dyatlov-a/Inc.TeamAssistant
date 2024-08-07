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
        
        var client = await _provider.Get(command.MessageContext.Bot.Id, token);
        var messages = _dialogContinuation.End(
            command.MessageContext.Bot.Id,
            command.MessageContext.TargetChat,
            command.SaveEndOfDialog ? null : command.MessageContext.ChatMessage);

        if (messages.Any())
            await client.TryDeleteMessages(command.MessageContext.Bot.Id, messages, _logger, token);
    }
}