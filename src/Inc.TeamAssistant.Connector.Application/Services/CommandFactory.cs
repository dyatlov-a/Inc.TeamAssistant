using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandFactory
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;
    private readonly DialogContinuation _dialogContinuation;
    private readonly DialogCommandFactory _dialogCommandFactory;

    public CommandFactory(
        IEnumerable<ICommandCreator> commandCreators,
        DialogContinuation dialogContinuation,
        DialogCommandFactory dialogCommandFactory)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _dialogCommandFactory = dialogCommandFactory ?? throw new ArgumentNullException(nameof(dialogCommandFactory));
    }

    public async Task<IRequest<CommandResult>?> TryCreate(
        ITelegramBotClient client,
        Bot bot,
        MessageContext messageContext,
        CancellationToken token)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));
        if (bot is null)
            throw new ArgumentNullException(nameof(bot));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var dialogState = _dialogContinuation.Find(messageContext.PersonId);
        var cmd = !CommandList.Cancel.Equals(messageContext.Text, StringComparison.InvariantCultureIgnoreCase) && dialogState is not null
            ? dialogState.Command
            : messageContext.Text;
        var botCommand = bot.FindCommand(cmd);
        
        if (botCommand?.Stages.Any() == true)
        {
            var stages = botCommand.Stages.ToArray();
            var firstStage = stages.First();

            for (var index = 0; index < stages.Length; index++)
            {
                var currentStage = stages[index];
                var nextIndex = index + 1;
                var nextStage = nextIndex < stages.Length ? stages[nextIndex] : null;
                
                if ((dialogState is null && firstStage.Id == currentStage.Id) || dialogState?.CommandState == currentStage.Value)
                {
                    var command = await _dialogCommandFactory.TryCreate(
                        bot,
                        botCommand.Value,
                        dialogState?.CommandState,
                        currentStage,
                        nextStage,
                        messageContext);

                    if (command is not null)
                        return command;
                }
            }
        }
        
        var commandCreator = _commandCreators.SingleOrDefault(c => cmd.StartsWith(
            c.Command,
            StringComparison.InvariantCultureIgnoreCase));

        if (commandCreator is not null)
        {
            var currentTeamContext = dialogState?.TeamContext ?? CurrentTeamContext.Empty;
            var command = await commandCreator.Create(messageContext, currentTeamContext, token);
            
            // TODO: move end dialog action to post processor
            
            await _dialogContinuation.End(
                messageContext.PersonId,
                new ChatMessage(messageContext.ChatId, messageContext.MessageId),
                async (ms, t) =>
                {
                    if (messageContext.Shared)
                        foreach (var m in ms)
                            await client.DeleteMessageAsync(m.ChatId, m.MessageId, t);
                },
                token);

            return command;
        }

        return null;
    }
}