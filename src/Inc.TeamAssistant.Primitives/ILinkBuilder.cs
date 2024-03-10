namespace Inc.TeamAssistant.Primitives;

public interface ILinkBuilder
{
    string BuildLinkMoveToBot(string botName);

    string BuildLinkForConnect(string botName, Guid teamId);
}