using System.Data;
using Dapper;

namespace Inc.TeamAssistant.Holidays.Internal;

internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        parameter.DbType = DbType.DateTime;
        parameter.Value = value.ToDateTime(new(0, 0));
    }

    public override DateOnly Parse(object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return DateOnly.FromDateTime((DateTime)value);
    }
}