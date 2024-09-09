using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.RandomCoffee;

public sealed class RandomCoffeeHistoryWidgetFormModel
{
    public long? ChatId { get; set; }
    public DateOnly? Date { get; set; }

    private readonly List<ChatDto> _chats = new();
    public IReadOnlyCollection<ChatDto> Chats => _chats;

    private readonly List<RandomCoffeeHistoryDto> _historyItems = new();
    public IReadOnlyCollection<RandomCoffeeHistoryDto> HistoryItems => _historyItems;

    public RandomCoffeeHistoryWidgetFormModel Apply(Parameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        ChatId = parameters.ChatId;
        Date = parameters.History?.Items.FirstOrDefault()?.Created;
        
        _chats.Clear();
        _chats.AddRange(parameters.Chats.Items);

        _historyItems.Clear();
        if (parameters.History is not null)
            _historyItems.AddRange(parameters.History.Items);
        
        return this;
    }

    public sealed record Parameters(GetChatsResult Chats, long? ChatId, GetHistoryResult? History);
}