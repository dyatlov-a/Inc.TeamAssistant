using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Domain;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateBot;

internal sealed class CreateBotCommandHandler : IRequestHandler<CreateBotCommand>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentUserResolver _currentUserResolver;

    public CreateBotCommandHandler(IBotRepository botRepository, ICurrentUserResolver currentUserResolver)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentUserResolver = currentUserResolver ?? throw new ArgumentNullException(nameof(currentUserResolver));
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
    }
}