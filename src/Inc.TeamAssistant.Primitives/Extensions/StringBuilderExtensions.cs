using System.Text;

namespace Inc.TeamAssistant.Primitives.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AddIfHasValue(this StringBuilder builder, string? value)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        var result = string.IsNullOrWhiteSpace(value)
            ? builder
            : builder.AppendLine(value);
        return result;
    }

    public static StringBuilder AddItems<T>(
        this StringBuilder builder,
        IEnumerable<T> items,
        Action<StringBuilder, T> action)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in items)
            action(builder, item);
        
        return builder;
    }
}