using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
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
        
        var existsEntry = await _repository.Find(command.MessageContext.ChatMessage.ChatId, token);
        if (existsEntry is null || existsEntry.IsRefused())
            return CommandResult.Empty;

        var pollMessageId = existsEntry
            .CheckRights(command.MessageContext.Person.Id)
            .MoveToRefused();
        
        await _repository.Upsert(existsEntry, token);

        var languageId = await _teamAccessor.GetClientLanguage(
            command.MessageContext.Bot.Id,
            existsEntry.OwnerId,
            token);
        var refusedMessage = _messageBuilder.Build(
            Messages.RandomCoffee_RefusedForCoffee,
            languageId,
            CommandList.InviteForCoffee);
        var notifications = new List<NotificationMessage>
        {
            NotificationMessage.Create(existsEntry.ChatId, refusedMessage),
            NotificationMessage.Delete(command.MessageContext.ChatMessage)
        };
        if (pollMessageId.HasValue)
            notifications.Add(NotificationMessage.Delete(new(existsEntry.ChatId, pollMessageId.Value)));
        
        return CommandResult.Build(notifications.ToArray());
    }
}