using System.Collections.Concurrent;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class DialogContinuation
{
    private readonly ConcurrentDictionary<(Guid BotId, TargetChat Chat), DialogState> _store = new();

    public DialogState? Find(Guid botId, TargetChat targetChat)
    {
        ArgumentNullException.ThrowIfNull(targetChat);
        
        return _store.GetValueOrDefault((botId, targetChat));
    }

    public DialogState Begin(
        Guid botId,
        TargetChat targetChat,
        string command,
        CommandStage commandStage,
        ChatMessage chatMessage)
    {
        ArgumentNullException.ThrowIfNull(targetChat);
        ArgumentNullException.ThrowIfNull(chatMessage);
        
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));

        var key = (botId, targetChat);
        
        _store.AddOrUpdate(
            key,
            k => new DialogState(command, commandStage).Attach(chatMessage),
            (k, v) => v.MoveTo(commandStage).Attach(chatMessage));

        return _store[key];
    }

    public async Task End(
        Guid botId,
        TargetChat targetChat,
        ChatMessage chatMessage,
        Func<IReadOnlyCollection<ChatMessage>, CancellationToken, Task> cleanHistory,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(targetChat);
        ArgumentNullException.ThrowIfNull(chatMessage);
        ArgumentNullException.ThrowIfNull(cleanHistory);

        if (_store.Remove((botId, targetChat), out var dialogState))
        {
            dialogState.Attach(chatMessage);

            if (dialogState.ChatMessages.Any())
                await cleanHistory(dialogState.ChatMessages, token);
        }
    }
}