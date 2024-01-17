using Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class DialogCommandFactory
{
    private readonly IMessageBuilder _messageBuilder;

    public DialogCommandFactory(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<IRequest<CommandResult>?> TryCreate(
        Bot bot,
        string botCommand,
        CommandStage? currentStage,
        BotCommandStage stage,
        MessageContext messageContext)
    {
        if (bot is null)
            throw new ArgumentNullException(nameof(bot));
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));
        if (stage is null)
            throw new ArgumentNullException(nameof(stage));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));

        var teamSelected = Guid.TryParse(messageContext.Text.TrimStart('/'), out var teamId);

        return (botCommand, currentStage, messageContext.Teams.Count, teamSelected) switch
        {
            ("/cancel", _, _, _) => null,
            ("/new_team", null, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId: null, stage.DialogMessageId),
            ("/leave_team", null, 1, _)
                => new LeaveFromTeamCommand(messageContext, messageContext.Teams[0].Id),
            ("/leave_team", null, > 1, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, allTeams: false),
            ("/need_review", null, 0, _)
                => throw new ApplicationException($"Teams for user {messageContext.PersonId} was not found."),
            ("/need_review", null, 1, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, stage.DialogMessageId),
            ("/need_review", null, > 1, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, allTeams: true),
            ("/need_review", CommandStage.SelectTeam, _, true)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, stage.DialogMessageId),
            ("/add", null, 0, _)
                => throw new ApplicationException($"Teams for user {messageContext.PersonId} was not found."),
            ("/add", null, 1, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, stage.DialogMessageId),
            ("/add", null, > 1, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, allTeams: true),
            ("/add", CommandStage.SelectTeam, _, true)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, stage.DialogMessageId),
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
            TeamContext: null,
            botCommand,
            notification);
    }

    private async Task<IRequest<CommandResult>> CreateEnterTextCommand(
        Bot bot,
        string botCommand,
        MessageContext messageContext,
        Guid? teamId,
        MessageId messageId)
    {
        if (bot is null)
            throw new ArgumentNullException(nameof(bot));
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (messageId is null)
            throw new ArgumentNullException(nameof(messageId));

        var team = teamId.HasValue ? bot.Teams.Single(t => t.Id == teamId.Value) : null;
        var notification = NotificationMessage.Create(
            messageContext.ChatId,
            await _messageBuilder.Build(messageId, messageContext.LanguageId));
            
        return new BeginCommand(
            messageContext,
            CommandStage.EnterText,
            team is not null ? new CurrentTeamContext(team.Id, team.Properties) : null,
            botCommand,
            notification);
    }
}