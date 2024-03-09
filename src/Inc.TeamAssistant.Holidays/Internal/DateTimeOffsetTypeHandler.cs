using System.Data;
using Dapper;

namespace Inc.TeamAssistant.Holidays.Internal;

internal sealed class DateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));
        
        parameter.Value = value;
    }

    public override DateTimeOffset Parse(object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        
        return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
    }
}
