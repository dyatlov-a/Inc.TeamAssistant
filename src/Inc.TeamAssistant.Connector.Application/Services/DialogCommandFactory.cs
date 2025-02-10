using Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;
using Inc.TeamAssistant.Connector.Domain;
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
        StageType? dialogStage,
        ContextStage currentStage,
        ContextStage? nextStage,
        MessageContext messageContext)
    {
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentException.ThrowIfNullOrWhiteSpace(botCommand);
        ArgumentNullException.ThrowIfNull(currentStage);
        ArgumentNullException.ThrowIfNull(messageContext);
        
        var teamSelected = Guid.TryParse(messageContext.Text.TrimStart('/'), out var teamId);
        var memberOfTeams = messageContext.Teams
            .Where(t => t.UserInTeam)
            .ToArray();
        var ownerOfTeams = messageContext.Teams
            .Where(t => t.OwnerOfTeam)
            .ToArray();

        return (botCommand,
                dialogStage,
                messageContext.Teams.Count,
                memberOfTeams.Length,
                ownerOfTeams.Length,
                teamSelected)
            switch
        {
            (CommandList.AddLocation, null, _, _, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId: null, currentStage.DialogMessageId),
            (CommandList.NewTeam, null, _, _, _, _)
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId: null, currentStage.DialogMessageId),
            (CommandList.LeaveTeam, null, _, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, memberOfTeams),
            (CommandList.RemoveTeam, null, _, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, ownerOfTeams),
            (CommandList.MoveToFibonacci, null, _, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, ownerOfTeams),
            (CommandList.MoveToTShirts, null, _, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, ownerOfTeams),
            (CommandList.MoveToPowerOfTwo, null, _, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, ownerOfTeams),
            (CommandList.ChangeToRoundRobin, null, _, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, ownerOfTeams),
            (CommandList.ChangeToRandom, null, _, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, ownerOfTeams),
            (CommandList.NeedReview, null, 0, _, _, _)
                => ThrowTeamForUserNotFound(messageContext.Person.Id),
            (CommandList.NeedReview, null, 1, _, _, _) when nextStage is not null
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, nextStage.DialogMessageId),
            (CommandList.NeedReview, null, > 1, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.NeedReview, StageType.SelectTeam, _, _, _, true) when nextStage is not null
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, nextStage.DialogMessageId),
            (CommandList.AddStory, null, 0, _, _, _)
                => ThrowTeamForUserNotFound(messageContext.Person.Id),
            (CommandList.AddStory, null, 1, _, _, _) when nextStage is not null
                => await CreateEnterTextCommand(bot, botCommand, messageContext, messageContext.Teams[0].Id, nextStage.DialogMessageId),
            (CommandList.AddStory, null, > 1, _, _, _)
                => await CreateSelectTeamCommand(botCommand, messageContext, messageContext.Teams),
            (CommandList.AddStory, StageType.SelectTeam, _, _, _, true) when nextStage is not null
                => await CreateEnterTextCommand(bot, botCommand, messageContext, teamId, nextStage.DialogMessageId),
            _ => null
        };
    }

    private async Task<IDialogCommand> CreateSelectTeamCommand(
        string botCommand,
        MessageContext messageContext,
        IReadOnlyCollection<TeamContext> teams)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(botCommand);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teams);

        var cancelButtonText = await _messageBuilder.Build(Messages.Connector_Cancel, messageContext.LanguageId);
        var notification = NotificationMessage.Create(
            messageContext.ChatMessage.ChatId,
            await _messageBuilder.Build(Messages.Connector_SelectTeam, messageContext.LanguageId));
            
        foreach (var team in teams.OrderBy(t => t.Name))
            notification.WithButton(new Button(team.Name, $"/{team.Id:N}"));

        notification
            .SetButtonsInRow(2)
            .WithButton(new Button(cancelButtonText, CommandList.Cancel));
        
        return new BeginCommand(
            messageContext,
            StageType.SelectTeam,
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
        ArgumentException.ThrowIfNullOrWhiteSpace(botCommand);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(messageId);

        var cancelButtonText = await _messageBuilder.Build(Messages.Connector_Cancel, messageContext.LanguageId);
        var team = teamId.HasValue ? bot.Teams.Single(t => t.Id == teamId.Value) : null;
        var notification = NotificationMessage.Create(
            messageContext.ChatMessage.ChatId,
            await _messageBuilder.Build(messageId, messageContext.LanguageId));
        
        notification.WithButton(new Button(cancelButtonText, CommandList.Cancel));
            
        return new BeginCommand(
            messageContext,
            StageType.EnterText,
            team is not null
                ? new CurrentTeamContext(team.Id, team.Name, team.Properties, team.BotId)
                : CurrentTeamContext.Empty,
            botCommand,
            notification);
    }

    private IDialogCommand ThrowTeamForUserNotFound(long personId)
    {
        throw new TeamAssistantUserException(Messages.Connector_TeamForUserNotFound, personId);
    }
}