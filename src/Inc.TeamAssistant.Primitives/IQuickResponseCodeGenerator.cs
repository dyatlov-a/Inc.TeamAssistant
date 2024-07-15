namespace Inc.TeamAssistant.Primitives;

public interface IQuickResponseCodeGenerator
{
    string Generate(string data, string foreground, string background);
}