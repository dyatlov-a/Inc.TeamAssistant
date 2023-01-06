namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface ICommandProvider
{
    string GetCommand(Type commandType);
}