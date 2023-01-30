using System.Data;
using Dapper;
using Inc.TeamAssistant.CheckIn.Primitives;

namespace Inc.TeamAssistant.CheckIn.DataAccess.Postgres;

internal sealed class MapIdTypeHandler : SqlMapper.TypeHandler<MapId>
{
    public override void SetValue(IDbDataParameter parameter, MapId mapId)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));
        if (mapId is null)
            throw new ArgumentNullException(nameof(mapId));

        parameter.DbType = DbType.Guid;
        parameter.Value = mapId.Value;
    }

    public override MapId Parse(object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new((Guid)value);
    }
}