namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IQuickResponseCodeGenerator
{
    string Generate(string data);
}