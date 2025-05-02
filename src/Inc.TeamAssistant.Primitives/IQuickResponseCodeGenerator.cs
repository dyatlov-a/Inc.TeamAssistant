namespace Inc.TeamAssistant.Primitives;

public interface IQuickResponseCodeGenerator
{
    Task<string> Generate(string data, string foreground, string background, CancellationToken token);
}