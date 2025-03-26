using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
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
        
        var existsEntry = await _repository.Find(command.MessageContext.ChatMessage.ChatId, token);
        if (existsEntry is null || existsEntry.State == RandomCoffeeState.Refused)
            return CommandResult.Empty;
        
        existsEntry.MoveToRefused(command.MessageContext.Person.Id);

        await _repository.Upsert(existsEntry, token);

        var languageId = await _teamAccessor.GetClientLanguage(command.MessageContext.Bot.Id, existsEntry.OwnerId, token);
        var notification = NotificationMessage.Create(
            existsEntry.ChatId,
            await _messageBuilder.Build(Messages.RandomCoffee_RefusedForCoffee, languageId));
        
        return CommandResult.Build(notification, NotificationMessage.Delete(command.MessageContext.ChatMessage));
    }
}