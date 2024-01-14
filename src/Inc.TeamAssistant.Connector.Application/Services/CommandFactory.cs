using Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class CommandFactory
{
    private readonly IEnumerable<ICommandCreator> _commandCreators;
    private readonly DialogContinuation _dialogContinuation;
    private readonly IMessageBuilder _messageBuilder;

    public CommandFactory(
        IEnumerable<ICommandCreator> commandCreators,
        DialogContinuation dialogContinuation,
        IMessageBuilder messageBuilder)
    {
        _commandCreators = commandCreators ?? throw new ArgumentNullException(nameof(commandCreators));
        _dialogContinuation = dialogContinuation ?? throw new ArgumentNullException(nameof(dialogContinuation));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
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
        var cmd = dialogState is not null ? dialogState.Command : messageContext.Text;
        var botCommand = bot.FindCommand(cmd);

        if (botCommand is null)
            return null;
        
        if (botCommand.Stages.Any())
        {
            var firstStage = botCommand.Stages.First();
            
            foreach (var stage in botCommand.Stages)
            {
                if (dialogState is null && firstStage.Id == stage.Id || dialogState?.CommandState == stage.Value)
                {
                    var command = await TryCreateCommandFromDialog(
                        botCommand.Value,
                        dialogState?.CommandState,
                        stage,
                        messageContext);

                    if (command is not null)
                        return command;
                }
            }
        }
        
        var commandCreator = _commandCreators.SingleOrDefault(c => c.Command.StartsWith(
            botCommand.Value,
            StringComparison.InvariantCultureIgnoreCase));

        if (commandCreator is not null)
        {
            var command = await commandCreator.Create(messageContext, dialogState?.TeamId, token);
            
            // TODO: add validation
            
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

    private async Task<IRequest<CommandResult>?> TryCreateCommandFromDialog(
        string botCommand,
        CommandStage? currentStage,
        BotCommandStage stage,
        MessageContext messageContext)
    {
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));
        if (stage is null)
            throw new ArgumentNullException(nameof(stage));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var teamSelected = Guid.TryParse(messageContext.Text.TrimStart('/'), out var teamId);

        return (botCommand, currentStage, messageContext.Teams.Count, teamSelected) switch
        {
            ("/new_team", null, _, _)
                => await CreateEnterTextCommand(botCommand, messageContext, teamId: null, stage.DialogMessageId),
            ("/leave_team", null, 1, _)
                => new LeaveFromTeamCommand(messageContext, messageContext.Teams[0].Id),
            ("/leave_team", null, > 1, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, allTeams: false),
            ("/need_review", null, 0, _)
                => throw new ApplicationException($"Teams for user {messageContext.PersonId} was not found."),
            ("/need_review", null, 1, _)
                => await CreateEnterTextCommand(botCommand, messageContext, messageContext.Teams[0].Id, stage.DialogMessageId),
            ("/need_review", null, > 1, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, allTeams: true),
            ("/need_review", CommandStage.SelectTeam, _, true)
                => await CreateEnterTextCommand(botCommand, messageContext, teamId, stage.DialogMessageId),
            ("/add", null, 0, _)
                => throw new ApplicationException($"Teams for user {messageContext.PersonId} was not found."),
            ("/add", null, 1, _)
                => await CreateEnterTextCommand(botCommand, messageContext, messageContext.Teams[0].Id, stage.DialogMessageId),
            ("/add", null, > 1, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, allTeams: true),
            ("/add", CommandStage.SelectTeam, _, true)
                => await CreateEnterTextCommand(botCommand, messageContext, teamId, stage.DialogMessageId),
            _ => null
        };
    }

    private async Task<IRequest<CommandResult>> CreateSelectTeamCommand(
        string botCommand,
        MessageContext messageContext,
        bool allTeams)
    {
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
            
        var notification = NotificationMessage.Create(
            messageContext.ChatId,
            await _messageBuilder.Build(Messages.Connector_SelectTeam, messageContext.LanguageId));
            
        foreach (var team in messageContext.Teams.Where(t => allTeams || t.UserInTeam))
            notification.WithButton(new Button(team.Name, $"/{team.Id:N}"));
        
        return new BeginCommand(
            messageContext,
            CommandStage.SelectTeam,
            SelectedTeamId: null,
            botCommand,
            notification);
    }

    private async Task<IRequest<CommandResult>> CreateEnterTextCommand(
        string botCommand,
        MessageContext messageContext,
        Guid? teamId,
        MessageId messageId)
    {
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));
        
        var notification = NotificationMessage.Create(
            messageContext.ChatId,
            await _messageBuilder.Build(messageId, messageContext.LanguageId));
            
        return new BeginCommand(
            messageContext,
            CommandStage.EnterText,
            SelectedTeamId: teamId,
            botCommand,
            notification);
    }
}