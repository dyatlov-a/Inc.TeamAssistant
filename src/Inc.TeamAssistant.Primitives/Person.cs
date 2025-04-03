using System.Text;

namespace Inc.TeamAssistant.Primitives;

public record Person(long Id, string Name, string? Username)
{
    public static readonly Person Empty = new(0, string.Empty, null);
    
    public string DisplayName => Username?.Equals(Name, StringComparison.InvariantCultureIgnoreCase) == false
        ? $"{Name} ({Username})"
        : Name;

    public virtual string AvatarUrl => $"/photos/{Id}";

    public StringBuilder AddTo(StringBuilder builder, Action<Person, int>? attach = null)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            attach?.Invoke(this, builder.Length);
            builder.Append(Name);
        }
        else
            builder.Append($"@{Username}");

        return builder;
    }
}