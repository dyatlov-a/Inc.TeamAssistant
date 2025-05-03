using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Application.Services;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee;

internal sealed class InviteForCoffeeCommandHandler : IRequestHandler<InviteForCoffeeCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly PollBuilder _pollBuilder;

    public InviteForCoffeeCommandHandler(IRandomCoffeeRepository repository, PollBuilder pollBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pollBuilder = pollBuilder ?? throw new ArgumentNullException(nameof(pollBuilder));
    }

    public async Task<CommandResult> Handle(InviteForCoffeeCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var existsEntry = await _repository.Find(command.MessageContext.ChatMessage.ChatId, token);
        var entry = existsEntry ?? new RandomCoffeeEntry(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            command.MessageContext.ChatMessage.ChatId,
            command.MessageContext.ChatName!,
            command.MessageContext.Person.Id);
        
        entry
            .CheckRights(command.MessageContext.Person.Id)
            .MoveToWaiting(
                DateTimeOffset.UtcNow,
                command.MessageContext.Bot.GetVotingInterval());

        await _repository.Upsert(entry, token);
        
        var notifications = new[]
        {
            await _pollBuilder.Build(entry, token),
            NotificationMessage.Delete(command.MessageContext.ChatMessage)
        };
        
        return CommandResult.Build(notifications);
    }
}