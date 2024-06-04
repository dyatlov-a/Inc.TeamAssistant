using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class LinkBuilder : ILinkBuilder
{
    public string BuildLinkForConnect(string botName, Guid teamId) => $"https://t.me/{botName}?start={teamId:N}";
}