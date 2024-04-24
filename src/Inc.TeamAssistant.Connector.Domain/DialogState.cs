using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class DialogState
{
    private readonly List<ChatMessage> _chatMessages = new();
    public IReadOnlyCollection<ChatMessage> ChatMessages => _chatMessages;
    
    public string Command { get; private set; }
    public CommandStage CommandState { get; private set; }
    public CurrentTeamContext TeamContext { get; private set; }

    public DialogState(string command, CommandStage commandState)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));
        
        Command = command;
        CommandState = commandState;
        TeamContext = CurrentTeamContext.Empty;
    }

    public DialogState Attach(ChatMessage chatMessage)
    {
        ArgumentNullException.ThrowIfNull(chatMessage);

        if (_chatMessages.All(cm => cm != chatMessage))
            _chatMessages.Add(chatMessage);
        
        return this;
    }

    public DialogState MoveTo(CommandStage continuationState)
    {
        CommandState = continuationState;

        return this;
    }

    public DialogState SetTeam(CurrentTeamContext teamContext)
    {
        TeamContext = teamContext ?? throw new ArgumentNullException(nameof(teamContext));

        return this;
    }
}