using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Commands.UpdateBot;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.CommandHandlers.UpdateBot;

internal sealed class UpdateBotCommandHandler : IRequestHandler<UpdateBotCommand>
{
    private readonly IBotRepository _botRepository;
    private readonly ICurrentUserResolver _currentUserResolver;

    public UpdateBotCommandHandler(IBotRepository botRepository, ICurrentUserResolver currentUserResolver)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
        _currentUserResolver = currentUserResolver ?? throw new ArgumentNullException(nameof(currentUserResolver));
    }
    
    public async Task Handle(UpdateBotCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentUserId = _currentUserResolver.GetUserId();
        var bot = await _botRepository.FindById(command.Id, token);
        if (bot?.OwnerId != currentUserId)
            throw new ApplicationException($"User {currentUserId} has not access to bot {command.Id}.");
        
        bot
            .ChangeName(command.Name)
            .ChangeToken(command.Token)
            .ChangeFeatures(command.FeatureIds);

        foreach (var property in command.Properties)
            bot.ChangeProperty(property.Key, property.Value);
        
        await _botRepository.Upsert(bot, token);
    }
}