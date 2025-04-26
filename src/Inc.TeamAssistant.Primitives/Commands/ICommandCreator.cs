namespace Inc.TeamAssistant.Primitives.Commands;

public interface ICommandCreator
{
    Task<IDialogCommand?> TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext,
        CancellationToken token);
}