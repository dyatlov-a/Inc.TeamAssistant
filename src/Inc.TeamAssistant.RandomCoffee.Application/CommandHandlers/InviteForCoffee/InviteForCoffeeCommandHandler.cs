using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AttachPoll;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee;

internal sealed class InviteForCoffeeCommandHandler : IRequestHandler<InviteForCoffeeCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly RandomCoffeeOptions _options;

    public InviteForCoffeeCommandHandler(IRandomCoffeeRepository repository, RandomCoffeeOptions options)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<CommandResult> Handle(InviteForCoffeeCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var chatId = command.MessageContext.ChatId;
        var existsRandomCoffeeEntry = await _repository.Find(chatId, token);

        if (existsRandomCoffeeEntry is not null && command.OnDemand)
            return CommandResult.Empty;
        
        var randomCoffeeEntry = existsRandomCoffeeEntry ?? new RandomCoffeeEntry(command.MessageContext.BotId, chatId);
        
        randomCoffeeEntry.MoveToWaiting(_options.WaitingInterval);

        await _repository.Upsert(randomCoffeeEntry, token);

        var notifications = new[]
        {
            NotificationMessage
                .Create(randomCoffeeEntry.ChatId, "Желаете участвовать во встречах в этом спринте?")
                .WithOption("Да")
                .WithOption("Нет")
                .AddHandler((c, p) => new AttachPollCommand(c, randomCoffeeEntry.Id, p))
        };

        if (command.MessageContext.MessageId != 0)
        {
            NotificationMessage.Delete(new ChatMessage(
                command.MessageContext.ChatId,
                command.MessageContext.MessageId));
        }
        
        return CommandResult.Build(notifications);
    }
}