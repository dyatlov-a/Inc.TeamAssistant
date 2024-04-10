using System.Data;
using Dapper;

namespace Inc.TeamAssistant.Primitives.DataAccess.Internal;

internal sealed class DateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        parameter.Value = value;
    }

    public override DateTimeOffset Parse(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
    }
}
