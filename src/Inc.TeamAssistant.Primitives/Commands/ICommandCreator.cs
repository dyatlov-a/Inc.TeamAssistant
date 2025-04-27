namespace Inc.TeamAssistant.Primitives.Commands;

public interface ICommandCreator
{
    IDialogCommand? TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext);
}