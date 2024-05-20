namespace Inc.TeamAssistant.Primitives;

public interface ILinkBuilder
{
    string BuildLinkForConnect(string botName, Guid teamId);
}