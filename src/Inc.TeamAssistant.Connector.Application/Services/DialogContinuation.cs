using System.Collections.Concurrent;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class DialogContinuation
{
    private readonly ConcurrentDictionary<long, DialogState> _store = new();

    public DialogState? Find(long userId) => _store.GetValueOrDefault(userId);

    public DialogState Begin(long userId, string command, CommandStage commandStage, ChatMessage chatMessage)
    {
        ArgumentNullException.ThrowIfNull(chatMessage);
        
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));

        _store.AddOrUpdate(
            userId,
            k => new DialogState(command, commandStage).Attach(chatMessage),
            (k, v) => v.MoveTo(commandStage).Attach(chatMessage));

        return _store[userId];
    }

    public async Task End(
        long userId,
        ChatMessage chatMessage,
        Func<IReadOnlyCollection<ChatMessage>, CancellationToken, Task> cleanHistory,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(chatMessage);
        ArgumentNullException.ThrowIfNull(cleanHistory);

        if (_store.Remove(userId, out var dialogState))
        {
            dialogState.Attach(chatMessage);

            if (dialogState.ChatMessages.Any())
                await cleanHistory(dialogState.ChatMessages, token);
        }
    }
}