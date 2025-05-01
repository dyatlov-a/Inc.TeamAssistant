using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.RandomCoffee.Domain;
using Inc.TeamAssistant.RandomCoffee.Model.Commands.AttachPoll;

namespace Inc.TeamAssistant.RandomCoffee.Application.Services;

internal sealed class PollBuilder
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;

    public PollBuilder(IMessageBuilder messageBuilder, ITeamAccessor teamAccessor)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<NotificationMessage> Build(RandomCoffeeEntry entry, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(entry);
        
        var languageId = await _teamAccessor.GetClientLanguage(entry.BotId, entry.OwnerId, token);
        
        var pollNotification = NotificationMessage
            .Create(entry.ChatId, _messageBuilder.Build(Messages.RandomCoffee_Question, languageId))
            .WithOption(_messageBuilder.Build(Messages.RandomCoffee_Yes, languageId))
            .WithOption(_messageBuilder.Build(Messages.RandomCoffee_No, languageId))
            .WithHandler((c, mId, pId) => new AttachPollCommand(c, entry.Id, pId!, mId));

        return pollNotification;
    }
}