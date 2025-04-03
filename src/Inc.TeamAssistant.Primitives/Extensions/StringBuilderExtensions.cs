using System.Text;

namespace Inc.TeamAssistant.Primitives.Extensions;

public static class StringBuilderExtensions
{
    private const string DefaultSeparator = " ";
    
    public static StringBuilder AddSeparator(this StringBuilder builder, string separator = DefaultSeparator)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(separator);
        
        builder.Append(separator);

        return builder;
    }
}