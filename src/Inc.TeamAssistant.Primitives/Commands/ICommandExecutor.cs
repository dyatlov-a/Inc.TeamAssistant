namespace Inc.TeamAssistant.Primitives.Commands;

public interface ICommandExecutor
{
    Task Execute(IDialogCommand command, CancellationToken token);
}