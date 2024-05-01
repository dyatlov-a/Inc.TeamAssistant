using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Services.Internal;

internal sealed class LinkBuilder : ILinkBuilder
{
    public string BuildLinkMoveToBot(string botName) => $"https://t.me/{botName}";

    public string BuildLinkForConnect(string botName, Guid teamId) => $"{BuildLinkMoveToBot(botName)}?start={teamId:N}";
}