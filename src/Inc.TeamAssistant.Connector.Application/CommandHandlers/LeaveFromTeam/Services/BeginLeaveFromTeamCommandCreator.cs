using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.Connector.Model.Commands.Begin;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Services;

internal sealed class BeginLeaveFromTeamCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly string _command = "/leave_team";
    
    public int Priority => 4;

    public BeginLeaveFromTeamCommandCreator(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    public async Task<IRequest<CommandResult>?> Create(MessageContext messageContext)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.Cmd.Equals(_command, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!messageContext.Teams.Any())
                throw new ValidationException(new[]
                {
                    new ValidationFailure(
                        "Teams",
                        await _messageBuilder.Build(Messages.Connector_TeamNotFound, messageContext.LanguageId))
                });
            if (messageContext.Teams.Count == 1)
                return new LeaveFromTeamCommand(messageContext, messageContext.Teams[0].Id);
            
            var notification = NotificationMessage.Create(
                messageContext.ChatId,
                await _messageBuilder.Build(Messages.Connector_SelectTeam, messageContext.LanguageId));
            
            foreach (var team in messageContext.Teams)
                notification.WithButton(new Button(team.Name, $"/{team.Id:N}"));
            
            return new BeginCommand(messageContext, BotCommandStage.SelectTeam, _command, notification);
        }

        return null;
    }
}