using Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Connector.Application.Services;

internal sealed class DialogCommandFactory
{
    private readonly IMessageBuilder _messageBuilder;

    public DialogCommandFactory(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<IDialogCommand?> TryCreate(
        Bot bot,
        string botCommand,
        CommandStage? currentStage,
        BotCommandStage stage,
        BotCommandStage? nextStage,
        MessageContext messageContext)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(stage);
        ArgumentNullException.ThrowIfNull(messageContext);
        
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));
        
        var teamSelected = Guid.TryParse(messageContext.Text.TrimStart('/'), out var teamId);
        var memberOfTeams = messageContext.Teams
            .Where(t => t.UserInTeam)
            .ToArray();

        return (botCommand, currentStage, messageContext.Teams.Count, memberOfTeams.Length, teamSelected) switch
        {
            (CommandList.AddLocation, null, _, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId: null, stage.DialogMessageId),
            (CommandList.NewTeam, null, _, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId: null, stage.DialogMessageId),
            (CommandList.LeaveTeam, null, _, 1, _)
                => new LeaveFromTeamCommand(messageContext, memberOfTeams[0].Id),
            (CommandList.LeaveTeam, null, _, > 1, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, memberOfTeams),
            (CommandList.MoveToSp, null, 1, _, _)
                => new ChangeTeamPropertyCommand(messageContext, messageContext.Teams[0].Id, "storyType", "Scrum"),
            (CommandList.MoveToSp, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.MoveToTShirts, null, 1, _, _)
                => new ChangeTeamPropertyCommand(messageContext, messageContext.Teams[0].Id, "storyType", "Kanban"),
            (CommandList.MoveToTShirts, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.ChangeToRoundRobin, null, 1, _, _)
                => new ChangeTeamPropertyCommand(messageContext, messageContext.Teams[0].Id, "nextReviewerStrategy", "RoundRobin"),
            (CommandList.ChangeToRoundRobin, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.ChangeToRandom, null, 1, _, _)
                => new ChangeTeamPropertyCommand(messageContext, messageContext.Teams[0].Id, "nextReviewerStrategy", "Random"),
            (CommandList.ChangeToRandom, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.NeedReview, null, 0, _, _)
                => throw new TeamAssistantUserException(Messages.Connector_TeamForUserNotFound, messageContext.Person.Id),
            (CommandList.NeedReview, null, 1, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, nextStage?.DialogMessageId ?? stage.DialogMessageId),
            (CommandList.NeedReview, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.NeedReview, CommandStage.SelectTeam, _, _, true)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, stage.DialogMessageId),
            (CommandList.AddStory, null, 0, _, _)
                => throw new TeamAssistantUserException(Messages.Connector_TeamForUserNotFound, messageContext.Person.Id),
            (CommandList.AddStory, null, 1, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, nextStage?.DialogMessageId ?? stage.DialogMessageId),
            (CommandList.AddStory, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.AddStory, CommandStage.SelectTeam, _, _, true)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, stage.DialogMessageId),
            _ => null
        };
    }

    private async Task<IDialogCommand> CreateSelectTeamCommand(
        string botCommand,
        MessageContext messageContext,
        IReadOnlyCollection<TeamContext> teams)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teams);
        
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));

        var notification = NotificationMessage.Create(
            messageContext.ChatMessage.ChatId,
            await _messageBuilder.Build(Messages.Connector_SelectTeam, messageContext.LanguageId));
            
        foreach (var team in teams.OrderBy(t => t.Name))
            notification.WithButton(new Button(team.Name, $"/{team.Id:N}"));
        
        return new BeginCommand(
            messageContext,
            CommandStage.SelectTeam,
            CurrentTeamContext.Empty,
            botCommand,
            notification);
    }

    private async Task<IDialogCommand> CreateEnterTextCommand(
        Bot bot,
        string botCommand,
        MessageContext messageContext,
        Guid? teamId,
        MessageId messageId)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(messageId);
        
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));

        var team = teamId.HasValue ? bot.Teams.Single(t => t.Id == teamId.Value) : null;
        var notification = NotificationMessage.Create(
            messageContext.ChatMessage.ChatId,
            await _messageBuilder.Build(messageId, messageContext.LanguageId));
            
        return new BeginCommand(
            messageContext,
            CommandStage.EnterText,
            team is not null ? new CurrentTeamContext(team.Id, team.Properties) : CurrentTeamContext.Empty,
            botCommand,
            notification);
    }
}