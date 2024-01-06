using System.Data;
using Dapper;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.DataAccess;

internal sealed class LanguageIdTypeHandler : SqlMapper.TypeHandler<LanguageId>
{
    public override void SetValue(IDbDataParameter parameter, LanguageId? languageId)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));

        parameter.DbType = DbType.String;
        parameter.Value = languageId?.Value;
    }

    public override LanguageId Parse(object value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new((string)value);
    }
}