using Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.ChangeTeamProperty;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
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
        BotCommandStage? nextStage,
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
        var memberOfTeams = messageContext.Teams
            .Where(t => t.UserInTeam)
            .ToArray();

        return (botCommand, currentStage, messageContext.Teams.Count, memberOfTeams.Length, teamSelected) switch
        {
            (CommandList.Cancel, _, _, _, _) => null,
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
                => throw new TeamAssistantUserException(Messages.Connector_TeamForUserNotFound, messageContext.PersonId),
            (CommandList.NeedReview, null, 1, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, nextStage?.DialogMessageId ?? stage.DialogMessageId),
            (CommandList.NeedReview, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.NeedReview, CommandStage.SelectTeam, _, _, true)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, stage.DialogMessageId),
            (CommandList.AddStory, null, 0, _, _)
                => throw new TeamAssistantUserException(Messages.Connector_TeamForUserNotFound, messageContext.PersonId),
            (CommandList.AddStory, null, 1, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, nextStage?.DialogMessageId ?? stage.DialogMessageId),
            (CommandList.AddStory, null, > 1, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.AddStory, CommandStage.SelectTeam, _, _, true)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, stage.DialogMessageId),
            _ => null
        };
    }

    private async Task<IRequest<CommandResult>> CreateSelectTeamCommand(
        string botCommand,
        MessageContext messageContext,
        IReadOnlyCollection<TeamContext> teams)
    {
        if (string.IsNullOrWhiteSpace(botCommand))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(botCommand));
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        if (teams is null)
            throw new ArgumentNullException(nameof(teams));
            
        var notification = NotificationMessage.Create(
            messageContext.ChatId,
            await _messageBuilder.Build(Messages.Connector_SelectTeam, messageContext.LanguageId));
            
        foreach (var team in teams)
            notification.WithButton(new Button(team.Name, $"/{team.Id:N}"));
        
        return new BeginCommand(
            messageContext,
            CommandStage.SelectTeam,
            CurrentTeamContext.Empty,
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
            team is not null ? new CurrentTeamContext(team.Id, team.Properties) : CurrentTeamContext.Empty,
            botCommand,
            notification);
    }
}