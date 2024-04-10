using System.Data;
using Dapper;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives.DataAccess.Internal;

internal sealed class LanguageIdTypeHandler : SqlMapper.TypeHandler<LanguageId>
{
    public override void SetValue(IDbDataParameter parameter, LanguageId? languageId)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        parameter.DbType = DbType.String;
        parameter.Value = languageId?.Value;
    }

    public override LanguageId Parse(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new((string)value);
    }
}