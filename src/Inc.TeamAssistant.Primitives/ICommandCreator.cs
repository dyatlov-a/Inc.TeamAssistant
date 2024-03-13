namespace Inc.TeamAssistant.Primitives;

public interface ICommandCreator
{
    string Command { get; }
    
    Task<IEndDialogCommand> Create(MessageContext messageContext, CurrentTeamContext teamContext, CancellationToken token);
}