using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.RandomCoffee;

public sealed class RandomCoffeeHistoryWidgetFormModel
{
    public long? ChatId { get; set; }
    public DateOnly? Date { get; set; }
    public IReadOnlyCollection<ChatDto> Chats { get; set; } = Array.Empty<ChatDto>();
    public IReadOnlyCollection<RandomCoffeeHistoryDto> HistoryItems { get; set; } = Array.Empty<RandomCoffeeHistoryDto>();
}