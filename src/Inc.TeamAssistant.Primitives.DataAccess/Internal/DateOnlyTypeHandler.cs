using System.Data;
using Dapper;

namespace Inc.TeamAssistant.Primitives.DataAccess.Internal;

internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        parameter.Value = value.ToDateTime(new(0, 0));
    }

    public override DateOnly Parse(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return DateOnly.FromDateTime((DateTime)value);
    }
}