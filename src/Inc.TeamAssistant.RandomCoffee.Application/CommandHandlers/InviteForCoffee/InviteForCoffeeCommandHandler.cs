using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
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

    public InviteForCoffeeCommandHandler(
        IRandomCoffeeRepository repository,
        ITeamAccessor teamAccessor,
        IMessageBuilder messageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
    }

    public async Task<CommandResult> Handle(InviteForCoffeeCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var existsEntry = await _repository.Find(command.MessageContext.ChatMessage.ChatId, token);
        if (existsEntry?.AlreadyStarted(command.OnDemand) == true)
            return CommandResult.Empty;
        
        var entry = existsEntry ?? new RandomCoffeeEntry(
            Guid.NewGuid(),
            command.MessageContext.Bot.Id,
            command.MessageContext.ChatMessage.ChatId,
            command.MessageContext.ChatName!,
            command.MessageContext.Person.Id);
        
        entry.MoveToWaiting(
            DateTimeOffset.UtcNow,
            command.MessageContext.Bot.GetVotingInterval(),
            command.OnDemand ? command.MessageContext.Person.Id : null);

        await _repository.Upsert(entry, token);

        var languageId = await _teamAccessor.GetClientLanguage(command.MessageContext.Bot.Id, entry.OwnerId, token);
        var notification = NotificationMessage
            .Create(
                entry.ChatId,
                _messageBuilder.Build(Messages.RandomCoffee_Question, languageId))
            .WithOption(_messageBuilder.Build(Messages.RandomCoffee_Yes, languageId))
            .WithOption(_messageBuilder.Build(Messages.RandomCoffee_No, languageId))
            .AddHandler((c, p) => new AttachPollCommand(c, entry.Id, p));
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