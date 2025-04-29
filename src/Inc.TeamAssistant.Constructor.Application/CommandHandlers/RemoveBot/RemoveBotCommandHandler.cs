using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.RemoveBot;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.RemoveBot;

internal sealed class RemoveBotCommandHandler : IRequestHandler<RemoveBotCommand>
{
    private readonly IBotRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IBotListeners _botListeners;

    public RemoveBotCommandHandler(
        IBotRepository repository,
        IPersonResolver personResolver,
        IBotListeners botListeners)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _botListeners = botListeners ?? throw new ArgumentNullException(nameof(botListeners));
    }

    public async Task Handle(RemoveBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _personResolver.GetCurrentPerson();
        var bot = await command.Id.Required(_repository.Find, token);
        
        bot.CheckRights(currentPerson.Id);
        
        await _botListeners.Stop(bot.Id, token);
        await _repository.Remove(bot.Id, token);
    }
}