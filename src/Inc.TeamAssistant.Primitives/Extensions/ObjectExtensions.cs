using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Primitives.Extensions;

public static class ObjectExtensions
{
    public static async Task<T> Required<T, TKey>(
        this TKey id,
        Func<TKey, CancellationToken, Task<T?>> action,
        CancellationToken token)
        where TKey : notnull
        where T : class
    {
        ArgumentNullException.ThrowIfNull(action);

        var type = typeof(T).Name;
        var value = await action(id, token);

        if (value is null)
            throw new TeamAssistantException($"{type} by id {id} was not found.");

        return value;
    }
}