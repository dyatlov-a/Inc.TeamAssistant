using FluentValidation;
using FluentValidation.Results;
using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Services;

internal sealed class CreateTeamCommandCreator : ICommandCreator
{
    private readonly IMessageBuilder _messageBuilder;
    
    public string Command => "/new_team";

    public CreateTeamCommandCreator(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }
    
    public async Task<IRequest<CommandResult>> Create(
        MessageContext messageContext,
        Guid? selectedTeamId,
        CancellationToken token)
    {
        if (messageContext is null)
            throw new ArgumentNullException(nameof(messageContext));
        
        if (messageContext.Text.StartsWith("/"))
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    "Command",
                    await _messageBuilder.Build(Messages.Connector_EnterTextError, messageContext.LanguageId))
            });
            
        return new CreateTeamCommand(messageContext, messageContext.BotName, messageContext.Text);
    }
}