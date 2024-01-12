using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class DialogState
{
    private readonly List<ChatMessage> _chatMessages = new();
    public IReadOnlyCollection<ChatMessage> ChatMessages => _chatMessages;
    
    public string Command { get; private set; }
    
    public long UserId { get; private set; }
    public CommandStage CommandState { get; private set; }
    public Guid? TeamId { get; private set; }

    public DialogState(long userId, string command, CommandStage commandState)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));

        UserId = userId;
        Command = command;
        CommandState = commandState;
    }

    public DialogState Attach(ChatMessage chatMessage)
    {
        if (chatMessage is null)
            throw new ArgumentNullException(nameof(chatMessage));
        
        if (chatMessage.Shared)
            _chatMessages.Add(chatMessage);
        
        return this;
    }

    public DialogState MoveTo(CommandStage continuationState)
    {
        CommandState = continuationState;

        return this;
    }

    public DialogState SetTeamId(Guid value)
    {
        TeamId = value;

        return this;
    }
}