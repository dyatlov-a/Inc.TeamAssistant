namespace Inc.TeamAssistant.WebUI.Extensions;

public interface IJsFunction<TResult>
{
    string Identifier { get; }
    
    object?[]? Args { get; }
    
    Action? OnExecuted { get; }
}