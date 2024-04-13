using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandFactory
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;
    private readonly DialogContinuation _dialogContinuation;
    private readonly DialogCommandFactory _dialogCommandFactory;
    private readonly AliasService _aliasService;

    public CommandFactory(
        IEnumerable<ICommandCreator> commandCreators,
        DialogContinuation dialogContinuation,
        DialogCommandFactory dialogCommandFactory,
        AliasService aliasService)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _dialogCommandFactory = dialogCommandFactory ?? throw new ArgumentNullException(nameof(dialogCommandFactory));
        _aliasService = aliasService ?? throw new ArgumentNullException(nameof(aliasService));
    }

    public async Task<IDialogCommand?> TryCreate(Bot bot, MessageContext messageContext, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);

        var dialogState = _dialogContinuation.Find(messageContext.PersonId);
        var cmd = !CommandList.Cancel.Equals(messageContext.Text, StringComparison.InvariantCultureIgnoreCase) && dialogState is not null
            ? dialogState.Command
            : _aliasService.OverrideCommand(messageContext.Text);
        
        var botCommand = bot.FindCommand(cmd);
        if (botCommand is null)
            return null;
        
        // TODO: /nr LIT_DEV about this feature and link
        // TODO: /add LIT about this story and link
        
        if (botCommand.Stages.Any())
        {
            var stages = botCommand.Stages.ToArray();
            var firstStage = stages.First();

            for (var index = 0; index < stages.Length; index++)
            {
                var currentStage = stages[index];
                var nextIndex = index + 1;
                var nextStage = nextIndex < stages.Length ? stages[nextIndex] : null;
                
                if ((dialogState is null && firstStage.Id == currentStage.Id) ||
                    dialogState?.CommandState == currentStage.Value)
                {
                    var dialogCommand = await _dialogCommandFactory.TryCreate(
                        bot,
                        botCommand.Value,
                        dialogState?.CommandState,
                        currentStage,
                        nextStage,
                        messageContext);

                    if (dialogCommand is not null)
                        return dialogCommand;
                }
            }
        }
            
        var commandCreator = _commandCreators.SingleOrDefault(c => cmd.StartsWith(
            c.Command,
            StringComparison.InvariantCultureIgnoreCase));
        if (commandCreator is null)
            return null;
        
        var currentTeamContext = dialogState?.TeamContext ?? CurrentTeamContext.Empty;
        var command = await commandCreator.Create(messageContext, currentTeamContext, token);
        return command;
    }
}