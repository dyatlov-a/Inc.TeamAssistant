namespace Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;

public sealed record GetChatsResult(IReadOnlyCollection<ChatDto> Items);