using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.RefuseForCoffee;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.RefuseForCoffee;

public sealed class RefuseForCoffeeCommandHandler : IRequestHandler<RefuseForCoffeeCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IMessageBuilder _messageBuilder;

    public RefuseForCoffeeCommandHandler(
        IRandomCoffeeRepository repository, 
        ITeamAccessor teamAccessor, 
        IMessageBuilder messageBuilder) 
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(RefuseForCoffeeCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var existsRandomCoffeeEntry = await _repository.Find(command.MessageContext.ChatMessage.ChatId, token);
        
        if (existsRandomCoffeeEntry is null || existsRandomCoffeeEntry.Refused)
            return CommandResult.Empty;
        
        if (command.MessageContext.Person.Id != existsRandomCoffeeEntry.OwnerId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, command.MessageContext.Person.Id);
        
        var owner = await _teamAccessor.FindPerson(existsRandomCoffeeEntry.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantException($"Owner {existsRandomCoffeeEntry.OwnerId} was not found.");
        
        existsRandomCoffeeEntry.MoveToRefused();

        await _repository.Upsert(existsRandomCoffeeEntry, token);

        var languageId = await _teamAccessor.GetClientLanguage(command.MessageContext.Bot.Id, owner.Id, token);
        var notification = NotificationMessage
            .Create(
                existsRandomCoffeeEntry.ChatId,
                await _messageBuilder.Build(Messages.RandomCoffee_RefusedForCoffee, languageId));
        var notifications = new[]
        {
            notification,
            NotificationMessage.Delete(command.MessageContext.ChatMessage)
        };
        
        return CommandResult.Build(notifications);
    }
}