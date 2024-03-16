using System.Data;
using System.Text.Json;
using Dapper;

namespace Inc.TeamAssistant.RandomCoffee.DataAccess;

internal sealed class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        parameter.Value = value is null
            ? default
            : JsonSerializer.Serialize(value);
    }

    public override T? Parse(object value)
    {
        return value is string str && !string.IsNullOrWhiteSpace(str)
            ? JsonSerializer.Deserialize<T>(str)
            : default;
    }
}