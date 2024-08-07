namespace Inc.TeamAssistant.Primitives.Commands;

public interface ICommandCreator
{
    string Command { get; }
    bool SupportSingleLineMode { get; }
    
    Task<IEndDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token);
}