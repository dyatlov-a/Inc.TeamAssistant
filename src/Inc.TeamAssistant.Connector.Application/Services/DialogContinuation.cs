using System.Collections.Concurrent;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class DialogContinuation
{
    private readonly ConcurrentDictionary<long, DialogState> _store = new();

    public DialogState? Find(long userId) => _store.GetValueOrDefault(userId);

    public DialogState Begin(long userId, string command, CommandStage commandStage, ChatMessage chatMessage)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));
        if (chatMessage is null)
            throw new ArgumentNullException(nameof(chatMessage));

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
        if (chatMessage is null)
            throw new ArgumentNullException(nameof(chatMessage));
        if (cleanHistory is null)
            throw new ArgumentNullException(nameof(cleanHistory));
        
        if (_store.Remove(userId, out var dialogState))
        {
            dialogState.Attach(chatMessage);

            if (dialogState.ChatMessages.Any())
                await cleanHistory(dialogState.ChatMessages, token);
        }
    }
}