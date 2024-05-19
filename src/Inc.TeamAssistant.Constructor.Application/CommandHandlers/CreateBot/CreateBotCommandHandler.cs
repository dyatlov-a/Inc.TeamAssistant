using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Domain;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateBot;

internal sealed class CreateBotCommandHandler : IRequestHandler<CreateBotCommand>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentUserResolver _currentUserResolver;
    private readonly IBotListeners _botListeners;

    public CreateBotCommandHandler(
        IBotRepository botRepository,
        ICurrentUserResolver currentUserResolver,
        IBotListeners botListeners)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentUserResolver = currentUserResolver ?? throw new ArgumentNullException(nameof(currentUserResolver));
        _botListeners = botListeners ?? throw new ArgumentNullException(nameof(botListeners));
    }

    public async Task Handle(CreateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var bot = new Bot(
            Guid.NewGuid(),
            command.Name,
            command.Token,
            _currentUserResolver.GetUserId(),
            command.Properties,
            command.FeatureIds);
        
        await _botRepository.Upsert(bot, token);

        await _botListeners.Start(bot.Id);
    }
}