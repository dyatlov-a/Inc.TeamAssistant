using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Domain;
using Inc.TeamAssistant.Constructor.Model.Commands.CreateBot;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.CreateBot;

internal sealed class CreateBotCommandHandler : IRequestHandler<CreateBotCommand>
{
    private readonly IBotRepository _botRepository;

    public CreateBotCommandHandler(IBotRepository botRepository)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task Handle(CreateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var bot = new Bot(
            Guid.NewGuid(),
            command.Name,
            command.Token,
            command.OwnerId,
            command.Properties);

        foreach (var featureId in command.FeatureIds)
            bot.AddFeature(featureId);
        
        await _botRepository.Upsert(bot, token);
    }
}