using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
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
    private readonly IMessageBuilder _messageBuilder;
    private readonly IBotAccessor _botAccessor;

    public InviteForCoffeeCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        IMessageBuilder messageBuilder,
        IBotAccessor botAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task<CommandResult> Handle(InviteForCoffeeCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var existsRandomCoffeeEntry = await _repository.Find(command.MessageContext.ChatMessage.ChatId, token);
        if (existsRandomCoffeeEntry is not null && command.OnDemand)
            return CommandResult.Empty;
        
        var randomCoffeeEntry = existsRandomCoffeeEntry ?? new RandomCoffeeEntry(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            command.MessageContext.ChatMessage.ChatId,
            command.MessageContext.ChatName!,
            command.MessageContext.Person.Id);
        var botContext = await _botAccessor.GetBotContext(randomCoffeeEntry.BotId, token);
        var owner = await _teamAccessor.FindPerson(randomCoffeeEntry.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantException($"Owner {randomCoffeeEntry.OwnerId} was not found.");
        
        randomCoffeeEntry.MoveToWaiting(DateTimeOffset.UtcNow, botContext.GetVotingInterval());

        await _repository.Upsert(randomCoffeeEntry, token);

        var languageId = await _teamAccessor.GetClientLanguage(command.MessageContext.Bot.Id, owner.Id, token);
        var notification = NotificationMessage
            .Create(
                randomCoffeeEntry.ChatId,
                await _messageBuilder.Build(Messages.RandomCoffee_Question, languageId))
            .WithOption(await _messageBuilder.Build(Messages.RandomCoffee_Yes, languageId))
            .WithOption(await _messageBuilder.Build(Messages.RandomCoffee_No, languageId))
            .AddHandler((c, p) => new AttachPollCommand(c, randomCoffeeEntry.Id, p));
        var notifications = command.MessageContext.ChatMessage.OnlyChat
            ? [notification]
            : new[]
            {
                notification,
                NotificationMessage.Delete(command.MessageContext.ChatMessage)
            };
        
        return CommandResult.Build(notifications);
    }
}