using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AttachPoll;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.InviteForCoffee;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.CommandHandlers.InviteForCoffee;

internal sealed class InviteForCoffeeCommandHandler : IRequestHandler<InviteForCoffeeCommand, CommandResult>
{
    private readonly IRandomCoffeeRepository _repository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly RandomCoffeeOptions _options;
    private readonly IMessageBuilder _messageBuilder;

    public InviteForCoffeeCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        RandomCoffeeOptions options,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(InviteForCoffeeCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var chatId = command.MessageContext.ChatId;
        var existsRandomCoffeeEntry = await _repository.Find(chatId, token);

        if (existsRandomCoffeeEntry is not null && command.OnDemand)
            return CommandResult.Empty;
        
        var randomCoffeeEntry = existsRandomCoffeeEntry ?? new RandomCoffeeEntry(
            command.MessageContext.BotId,
            chatId,
            command.MessageContext.PersonId);
        var owner = await _teamAccessor.FindPerson(randomCoffeeEntry.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantException($"Owner {randomCoffeeEntry.OwnerId} was not found.");
        
        randomCoffeeEntry.MoveToWaiting(_options.WaitingInterval);

        await _repository.Upsert(randomCoffeeEntry, token);

        var languageId = owner.GetLanguageId();
        var notifications = new[]
        {
            NotificationMessage
                .Create(randomCoffeeEntry.ChatId, await _messageBuilder.Build(Messages.RandomCoffee_Question, languageId))
                .WithOption(await _messageBuilder.Build(Messages.RandomCoffee_Yes, languageId))
                .WithOption(await _messageBuilder.Build(Messages.RandomCoffee_No, languageId))
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