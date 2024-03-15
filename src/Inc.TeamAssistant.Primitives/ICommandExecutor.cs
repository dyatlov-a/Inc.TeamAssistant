namespace Inc.TeamAssistant.Primitives;

public interface ICommandExecutor
{
    Task Execute(IDialogCommand command, CancellationToken token);
}