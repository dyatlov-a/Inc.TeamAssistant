namespace Inc.TeamAssistant.Primitives.Commands;

public interface IEndDialogCommand : IDialogCommand
{ 
    bool SaveEndOfDialog => false;
}