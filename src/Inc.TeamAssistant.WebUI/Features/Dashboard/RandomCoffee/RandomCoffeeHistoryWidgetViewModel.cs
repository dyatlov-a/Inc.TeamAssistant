using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetHistory;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.RandomCoffee;

public sealed record RandomCoffeeHistoryWidgetViewModel(
    string FirstTitle,
    string SecondTitle,
    string NoPair,
    IReadOnlyCollection<ChatDto> Chats,
    IReadOnlyCollection<RandomCoffeeHistoryDto> HistoryItems)
    : IViewModel<RandomCoffeeHistoryWidgetViewModel>
{
    public static RandomCoffeeHistoryWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<ChatDto>(),
        Array.Empty<RandomCoffeeHistoryDto>());
}