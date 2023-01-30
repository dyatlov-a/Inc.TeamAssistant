using System.Data;
using Dapper;
using Inc.TeamAssistant.CheckIn.Primitives;

namespace Inc.TeamAssistant.CheckIn.DataAccess.Postgres;

internal sealed class LocationOnMapIdTypeHandler : SqlMapper.TypeHandler<LocationOnMapId>
{
    public override void SetValue(IDbDataParameter parameter, LocationOnMapId locationOnMapId)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));
        if (locationOnMapId is null)
            throw new ArgumentNullException(nameof(locationOnMapId));

        parameter.DbType = DbType.Guid;
        parameter.Value = locationOnMapId.Value;
    }

    public override LocationOnMapId Parse(object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new((Guid)value);
    }
}