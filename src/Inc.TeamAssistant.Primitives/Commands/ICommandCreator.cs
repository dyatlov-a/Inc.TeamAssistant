namespace Inc.TeamAssistant.Primitives.Commands;

public interface ICommandCreator
{
    string Command { get; }
    bool SupportSingleLineMode => false;
    
    Task<IDialogCommand> Create(
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token);
}